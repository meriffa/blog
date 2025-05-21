using ByteZoo.Blog.Common.MemoryMap.Records;
using ByteZoo.Blog.Common.MemoryMap.Services;
using ByteZoo.Blog.Common.Services;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Memory map viewer controller
/// </summary>
[Verb("Tools-MemoryMap", HelpText = "Memory map viewer operation.")]
public partial class MemoryMapController : Controller
{

    #region Properties
    /// <summary>
    /// Target process id
    /// </summary>
    [Option('p', "processId", SetName = "Live", Required = true, HelpText = "Target process id.")]
    public int ProcessId { get; set; }

    /// <summary>
    /// Target dump file
    /// </summary>
    [Option('d', "dumpFile", SetName = "Dump", Required = true, HelpText = "Target dump file.")]
    public string? DumpFile { get; set; }

    /// <summary>
    /// Path include expression
    /// </summary>
    [Option('i', "include", HelpText = "Path include expression.")]
    public string? PathIncludeExpression { get; set; }

    /// <summary>
    /// Path exclude expression
    /// </summary>
    [Option('e', "exclude", HelpText = "Path exclude expression.")]
    public string? PathExcludeExpression { get; set; }

    /// <summary>
    /// Filter regions by address
    /// </summary>
    [Option('a', "address", HelpText = "Filter regions by address.")]
    public string? Address { get; set; }

    /// <summary>
    /// Filter regions by start address
    /// </summary>
    [Option("startAddress", HelpText = "Filter regions by start address.")]
    public string? StartAddress { get; set; }

    /// <summary>
    /// Filter regions by end address
    /// </summary>
    [Option("endAddress", HelpText = "Filter regions by end address.")]
    public string? EndAddress { get; set; }

    /// <summary>
    /// Group regions by path
    /// </summary>
    [Option('g', "group", HelpText = "Group regions by path.")]
    public bool GroupByPath { get; set; }

    /// <summary>
    /// Display regions
    /// </summary>
    [Option('r', "regions", HelpText = "Display path regions.")]
    public bool IncludeRegions { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var regions = string.IsNullOrEmpty(DumpFile) ? new ProcessService(ProcessId).GetMemoryRegions() : new DumpService(DumpFile).GetMemoryRegions();
        if (!string.IsNullOrEmpty(PathIncludeExpression))
            regions = MemoryMapService.FilterMemoryRegionsByPath(regions, PathIncludeExpression, true);
        if (!string.IsNullOrEmpty(PathExcludeExpression))
            regions = MemoryMapService.FilterMemoryRegionsByPath(regions, PathExcludeExpression, false);
        if (!string.IsNullOrEmpty(Address))
            regions = MemoryMapService.FilterMemoryRegionsByAddress(regions, Convert.ToUInt64(Address, 16));
        if (!string.IsNullOrEmpty(StartAddress))
            regions = MemoryMapService.FilterMemoryRegionsByStartAddress(regions, Convert.ToUInt64(StartAddress, 16));
        if (!string.IsNullOrEmpty(EndAddress))
            regions = MemoryMapService.FilterMemoryRegionsByEndAddress(regions, Convert.ToUInt64(EndAddress, 16));
        if (GroupByPath)
            DisplayRegions(MemoryMapService.GetMemoryRegionGroups(regions), regions, IncludeRegions);
        else
            DisplayRegions(regions);
    }
    #endregion

    #region Private Members
    /// <summary>
    /// Display group regions
    /// </summary>
    /// <param name="groups"></param>
    /// <param name="regions"></param>
    /// <param name="includeRegions"></param>
    private static void DisplayRegions(List<MemoryRegionGroup> groups, List<MemoryRegion> regions, bool includeRegions)
    {
        foreach (var group in groups.OrderBy(i => i.Order))
        {
            DisplayService.WriteText($"- Path: File = '{group.Path}', Size = {MemoryMapService.FormatSize(group.Size)}");
            if (includeRegions)
            {
                var groupRegions = regions.Where(i => i.Path == group.Path).OrderBy(i => i.Start);
                foreach (var region in groupRegions)
                    DisplayService.WriteText($"  - Region: {MemoryMapService.FormatAddress(region.Start)}-{MemoryMapService.FormatAddress(region.End)}, Size = {MemoryMapService.FormatSize(region.Size)}, Permissions = {MemoryMapService.FormatPermissions(region.Permissions)}");
                var groupTotal = MemoryMapService.GetMemoryRegionsTotal(groupRegions);
                DisplayService.WriteText($"  - Region Total: Count = {groupTotal.Count}, Size = {MemoryMapService.FormatSize(groupTotal.Size)}");
            }
        }
        var pathTotal = MemoryMapService.GetMemoryRegionGroupsTotal(groups);
        DisplayService.WriteText($"- Path Total: Count = {pathTotal.Count}, Size = {MemoryMapService.FormatSize(pathTotal.Size)}");
    }

    /// <summary>
    /// Display regions
    /// </summary>
    /// <param name="regions"></param>
    private static void DisplayRegions(List<MemoryRegion> regions)
    {
        foreach (var region in regions.OrderBy(i => i.Start))
            DisplayService.WriteText($"- Region: {MemoryMapService.FormatAddress(region.Start)}-{MemoryMapService.FormatAddress(region.End)}, Size = {MemoryMapService.FormatSize(region.Size)}, Permissions = {MemoryMapService.FormatPermissions(region.Permissions)}, File = '{region.Path}'");
        var total = MemoryMapService.GetMemoryRegionsTotal(regions);
        DisplayService.WriteText($"- Total: Count = {total.Count}, Size = {MemoryMapService.FormatSize(total.Size)}");
    }
    #endregion

}