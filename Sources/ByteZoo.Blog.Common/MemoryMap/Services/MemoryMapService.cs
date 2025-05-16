using ByteZoo.Blog.Common.MemoryMap.Enums;
using ByteZoo.Blog.Common.MemoryMap.Records;
using System.Text;
using System.Text.RegularExpressions;

namespace ByteZoo.Blog.Common.MemoryMap.Services;

/// <summary>
/// Memory map service
/// </summary>
public abstract class MemoryMapService
{

    #region Constants
    protected const long KB = 1024L;
    protected const long MB = 1024L * 1024L;
    protected const long GB = 1024L * 1024L * 1024L;
    protected const string Anonymous = "[anonymous]";
    #endregion

    #region Public Methods
    /// <summary>
    /// Return memory regions
    /// </summary>
    /// <returns></returns>
    public abstract List<MemoryRegion> GetMemoryRegions();

    /// <summary>
    /// Filter memory regions
    /// </summary>
    /// <param name="regions"></param>
    /// <param name="address"></param>
    /// <returns></returns>
    public static List<MemoryRegion> FilterMemoryRegions(List<MemoryRegion> regions, long address) => [.. regions.Where(i => i.Start <= address && address <= i.End)];

    /// <summary>
    /// Return memory regions total
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    public static MemoryRegionTotal GetMemoryRegionsTotal(IEnumerable<MemoryRegion> regions) => new(Count: regions.Count(), Vss: regions.Sum(i => i.Vss), Rss: regions.Sum(i => i.Rss), Pss: regions.Sum(i => i.Pss), Uss: regions.Sum(i => i.Uss));

    /// <summary>
    /// Return memory region groups
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    public static List<MemoryRegionGroup> GetMemoryRegionGroups(List<MemoryRegion> regions) => [.. regions.GroupBy(i => i.Path).Select(i => new MemoryRegionGroup(Path: i.Key, Vss: i.Sum(i => i.Vss), Rss: i.Sum(i => i.Rss), Pss: i.Sum(i => i.Pss), Uss: i.Sum(i => i.Uss), Order: i.Min(i => i.Start)))];

    /// <summary>
    /// Filter memory region groups
    /// </summary>
    /// <param name="groups"></param>
    /// <param name="pathExpression"></param>
    /// <param name="IsMatch"></param>
    /// <returns></returns>
    public static List<MemoryRegionGroup> FilterMemoryRegionGroups(List<MemoryRegionGroup> groups, string pathExpression, bool IsMatch)
    {
        var expression = new Regex(pathExpression);
        return IsMatch ? [.. groups.Where(i => expression.IsMatch(i.Path))] : [.. groups.Where(i => !expression.IsMatch(i.Path))];
    }

    /// <summary>
    /// Return memory region groups total
    /// </summary>
    /// <param name="groups"></param>
    /// <returns></returns>
    public static MemoryRegionTotal GetMemoryRegionGroupsTotal(IEnumerable<MemoryRegionGroup> groups) => new(Count: groups.Count(), Vss: groups.Sum(i => i.Vss), Rss: groups.Sum(i => i.Rss), Pss: groups.Sum(i => i.Pss), Uss: groups.Sum(i => i.Uss));

    /// <summary>
    /// Format address
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatAddress(long value) => $"{value:X16}";

    /// <summary>
    /// Return size value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatSize(long value)
    {
        if (value < MB)
            return $"{value / KB} KB";
        else if (value < GB)
            return $"{((decimal)value) / MB:n} MB";
        else
            return $"{(decimal)value / GB:n} GB";
    }

    /// <summary>
    /// Format permissions
    /// </summary>
    /// <param name="permissions"></param>
    /// <returns></returns>
    public static string FormatPermissions(MemoryRegionPermissions permissions)
    {
        var result = new StringBuilder();
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
        return result.Length == 0 ? string.Empty : result.ToString(0, result.Length - 2);
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
    #endregion

}