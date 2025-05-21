using ByteZoo.Blog.Common.MemoryMap.Enums;
using ByteZoo.Blog.Common.MemoryMap.Records;
using Microsoft.Diagnostics.Runtime;
using System.Text;
using System.Text.RegularExpressions;

namespace ByteZoo.Blog.Common.MemoryMap.Services;

/// <summary>
/// Memory map service
/// </summary>
public abstract class MemoryMapService
{

    #region Constants
    protected const ulong KB = 1024UL;
    protected const string Anonymous = "[anonymous]";
    #endregion

    #region Public Methods
    /// <summary>
    /// Return memory regions
    /// </summary>
    /// <returns></returns>
    public abstract List<MemoryRegion> GetMemoryRegions();

    /// <summary>
    /// Filter memory regions by path
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="pathExpression"></param>
    /// <param name="IsMatch"></param>
    /// <returns></returns>
    public static List<MemoryRegion> FilterMemoryRegionsByPath(List<MemoryRegion> regions, string pathExpression, bool IsMatch)
    {
        var expression = new Regex(pathExpression);
        return IsMatch ? [.. regions.Where(i => expression.IsMatch(i.Path))] : [.. regions.Where(i => !expression.IsMatch(i.Path))];
    }

    /// <summary>
    /// Filter memory regions by address
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="address"></param>
    /// <returns></returns>
    public static List<MemoryRegion> FilterMemoryRegionsByAddress(List<MemoryRegion> regions, ulong address) => [.. regions.Where(i => i.Start <= address && address <= i.End)];

    /// <summary>
    /// Filter memory regions by start address
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="address"></param>
    /// <returns></returns>
    public static List<MemoryRegion> FilterMemoryRegionsByStartAddress(List<MemoryRegion> regions, ulong address) => [.. regions.Where(i => address <= i.End)];

    /// <summary>
    /// Filter memory regions by end address
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="address"></param>
    /// <returns></returns>
    public static List<MemoryRegion> FilterMemoryRegionsByEndAddress(List<MemoryRegion> regions, ulong address) => [.. regions.Where(i => i.Start <= address)];

    /// <summary>
    /// Return memory regions total
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    public static MemoryRegionTotal GetMemoryRegionsTotal(IEnumerable<MemoryRegion> regions) => new(Count: regions.Count(), Size: regions.Aggregate(0UL, (i, j) => i + j.Size));

    /// <summary>
    /// Return memory region groups
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    public static List<MemoryRegionGroup> GetMemoryRegionGroups(List<MemoryRegion> regions) => [.. regions.GroupBy(i => i.Path).Select(i => new MemoryRegionGroup(Path: i.Key, Size: i.Aggregate(0UL, (i, j) => i + j.Size), Order: i.Min(i => i.Start)))];

    /// <summary>
    /// Return memory region groups total
    /// </summary>
    /// <param name="groups"></param>
    /// <returns></returns>
    public static MemoryRegionTotal GetMemoryRegionGroupsTotal(IEnumerable<MemoryRegionGroup> groups) => new(Count: groups.Count(), Size: groups.Aggregate(0UL, (i, j) => i + j.Size));

    /// <summary>
    /// Format address
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatAddress(ulong value) => $"{value:X16}";

    /// <summary>
    /// Return size value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatSize(ulong value) => $"{value / KB} KB";

    /// <summary>
    /// Format permissions
    /// </summary>
    /// <param name="permissions"></param>
    /// <returns></returns>
    public static string FormatPermissions(MemoryRegionPermissions permissions)
    {
        var result = new StringBuilder();
        result.Append('{');
        if (permissions.HasFlag(MemoryRegionPermissions.Read))
            result.Append("Read").Append(", ");
        if (permissions.HasFlag(MemoryRegionPermissions.Write))
            result.Append("Write").Append(", ");
        if (permissions.HasFlag(MemoryRegionPermissions.Execute))
            result.Append("Execute").Append(", ");
        if (permissions.HasFlag(MemoryRegionPermissions.Private))
            result.Append("Private").Append(", ");
        if (permissions.HasFlag(MemoryRegionPermissions.Shared))
            result.Append("Shared").Append(", ");
        if (result.Length > 1)
            result.Remove(result.Length - 2, 2);
        result.Append('}');
        return result.ToString();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return memory region permissions
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static MemoryRegionPermissions GetMemoryRegionPermissions(string value)
    {
        var permissions = MemoryRegionPermissions.None;
        if (value.Contains('r'))
            permissions |= MemoryRegionPermissions.Read;
        if (value.Contains('w'))
            permissions |= MemoryRegionPermissions.Write;
        if (value.Contains('x') || value.Contains('e'))
            permissions |= MemoryRegionPermissions.Execute;
        if (value.Contains('p'))
            permissions |= MemoryRegionPermissions.Private;
        if (value.Contains('s'))
            permissions |= MemoryRegionPermissions.Shared;
        return permissions;
    }

    /// <summary>
    /// Update core dump memory regions
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="mappings"></param>
    /// <returns></returns>
    protected static List<MemoryRegion> UpdateMemoryRegions(List<MemoryRegion> regions, List<MemoryRegionMapping> mappings)
    {
        foreach (var mapping in mappings)
        {
            // Regions fully contained withing the mapping
            var fullyContainedRegions = regions.Where(i => mapping.Start <= i.Start && i.End <= mapping.End);
            if (fullyContainedRegions.Any())
                foreach (var region in fullyContainedRegions)
                    UpdateMemoryRegionPath(region, mapping);
            else
            {
                // Region containing the entire mapping
                var containingRegion = regions.FirstOrDefault(i => i.Start <= mapping.Start && mapping.End <= i.End);
                if (containingRegion != null)
                    SplitMemoryRegionAndUpdateMemoryRegion(regions, containingRegion, mapping);
                else
                {
                    // Regions partially overlapping with the mapping
                    var overlappingRegions = regions.Where(i => (i.Start <= mapping.Start && mapping.Start <= i.End) || (i.Start <= mapping.End && mapping.End <= i.End));
                    if (overlappingRegions.Any())
                        foreach (var overlappingRegion in overlappingRegions.ToArray())
                            SplitMemoryRegionAndUpdateMemoryRegion(regions, overlappingRegion, mapping);
                    else
                        throw new($"Unresolved region mapping found (Path = {mapping.Path}, Start = {mapping.Start:X16}, End = {mapping.End:X16}).");
                }
            }
        }
        return regions;
    }

    /// <summary>
    /// Consolidate adjacent memory regions
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    protected static List<MemoryRegion> ConsolidateMemoryRegions(List<MemoryRegion> regions)
    {
        var orderedRegions = regions.OrderBy(i => i.Start).ToArray();
        if (orderedRegions.Length > 1)
        {
            for (int i = 0; i < orderedRegions.Length - 1; i++)
                if (orderedRegions[i].End >= orderedRegions[i + 1].Start)
                    throw new("Overlapping memory regions found.");
            var consolidatedRegions = new List<MemoryRegion>();
            for (int i = 0; i < orderedRegions.Length - 1; i++)
            {
                var start = i;
                while (orderedRegions[i].End + 1UL == orderedRegions[i + 1].Start && orderedRegions[i].Permissions == orderedRegions[i + 1].Permissions && orderedRegions[i].Path == orderedRegions[i + 1].Path && i < orderedRegions.Length - 1)
                    i++;
                if (start != i)
                    consolidatedRegions.Add(orderedRegions[start] with { End = orderedRegions[i].End });
                else
                    consolidatedRegions.Add(orderedRegions[start]);
            }
        }
        return regions;
    }

    /// <summary>
    /// Return target instance
    /// </summary>
    /// <returns></returns>
    protected abstract DataTarget GetDataTarget();

    /// <summary>
    /// Update CLR memory regions
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    protected List<MemoryRegion> UpdateClrMemoryRegions(List<MemoryRegion> regions)
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        UpdateClrManagedHeapSegments(runtime, regions);
        UpdateClrNativeHeapSegments(runtime, regions);
        return regions;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Update memory region path
    /// </summary>
    /// <param name="region"></param>
    /// <param name="mapping"></param>
    private static void UpdateMemoryRegionPath(MemoryRegion region, MemoryRegionMapping mapping)
    {
        if (region.Path != Anonymous && !IsDeletedMemoryRegion(region.Path))
            throw new($"Duplicate region mapping found (Path = {mapping.Path}, Start = {mapping.Start:X16}, End = {mapping.End:X16}).");
        region.Path = mapping.Path;
    }

    /// <summary>
    /// Check if memory region is deleted
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool IsDeletedMemoryRegion(string path) => path == "/memfd:doublemapper (deleted)";

    /// <summary>
    /// Split memory region and update memory regions
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="region"></param>
    /// <param name="mapping"></param>
    private static void SplitMemoryRegionAndUpdateMemoryRegion(List<MemoryRegion> regions, MemoryRegion region, MemoryRegionMapping mapping)
    {
        (var subRegions, var index) = SplitMemoryRegion(region, mapping);
        UpdateMemoryRegionPath(subRegions[index], mapping);
        regions.Remove(region);
        regions.AddRange(subRegions);
    }

    /// <summary>
    /// Split memory region
    /// </summary>
    /// <param name="region"></param>
    /// <param name="mapping"></param>
    /// <returns></returns>
    private static (MemoryRegion[], int) SplitMemoryRegion(MemoryRegion region, MemoryRegionMapping mapping)
    {
        if (region.Start == mapping.Start && mapping.End < region.End)
            return ([region with { End = mapping.End }, region with { Start = mapping.End + 1UL }], 0);
        else if (region.Start < mapping.Start && mapping.End == region.End)
            return ([region with { End = mapping.Start - 1UL }, region with { Start = mapping.Start }], 1);
        else if (region.Start < mapping.Start && mapping.End < region.End)
            return ([region with { End = mapping.Start - 1UL }, region with { Start = mapping.Start, End = mapping.End }, region with { Start = mapping.End + 1UL }], 1);
        else if (region.Start < mapping.Start && mapping.Start <= region.End)
            return ([region with { End = mapping.Start - 1UL }, region with { Start = mapping.Start }], 1);
        else if (region.Start < mapping.End && mapping.End <= region.End)
            return ([region with { End = mapping.End }, region with { Start = mapping.End + 1UL }], 0);
        else
            throw new("Invalid memory region split type.");
    }

    /// <summary>
    /// Return CLR runtime instance
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private static ClrRuntime GetClrRuntime(DataTarget target) => target.ClrVersions[0].CreateRuntime();

    /// <summary>
    /// Update CLR managed heap segments
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="regions"></param>
    private static void UpdateClrManagedHeapSegments(ClrRuntime runtime, List<MemoryRegion> regions)
    {
        var mappings = new List<MemoryRegionMapping>();
        foreach (var heap in runtime.Heap.SubHeaps)
            foreach (var segment in heap.Segments)
                mappings.Add(new(Path: $"[GC Heap #{heap.Index} ({segment.Kind})]", Start: segment.CommittedMemory.Start, End: segment.CommittedMemory.End - 1UL));
        UpdateMemoryRegions(regions, mappings);
    }

    /// <summary>
    /// Update CLR native heap segments
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="regions"></param>
    private static void UpdateClrNativeHeapSegments(ClrRuntime runtime, List<MemoryRegion> regions)
    {
        var mappings = new List<MemoryRegionMapping>();
        foreach (var heap in runtime.EnumerateClrNativeHeaps().Where(i => i.State == ClrNativeHeapState.Active || i.State == ClrNativeHeapState.RegionOfRegions))
        {
            var mapping = new MemoryRegionMapping(Path: $"[CLR Heap ({heap.Kind})]", Start: heap.MemoryRange.Start, End: heap.MemoryRange.End - 1UL);
            if (!mappings.Exists(i => i.Path == mapping.Path && i.Start == mapping.Start && i.End == mapping.End))
                mappings.Add(mapping);
        }
        UpdateMemoryRegions(regions, mappings);
    }
    #endregion

}