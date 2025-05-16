using ByteZoo.Blog.Common.MemoryMap.Services;
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
    /// Display regions
    /// </summary>
    [Option('r', "displayRegions", HelpText = "Display regions.")]
    public bool DisplayRegions { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        MemoryMapService service = string.IsNullOrEmpty(DumpFile) ? new ProcessService(ProcessId) : new DumpService(DumpFile);
        var regions = service.GetMemoryRegions();
        if (!string.IsNullOrEmpty(Address))
            regions = MemoryMapService.FilterMemoryRegions(regions, Convert.ToInt64(Address, 16));
        var groups = MemoryMapService.GetMemoryRegionGroups(regions);
        if (!string.IsNullOrEmpty(PathIncludeExpression))
            groups = MemoryMapService.FilterMemoryRegionGroups(groups, PathIncludeExpression, true);
        if (!string.IsNullOrEmpty(PathExcludeExpression))
            groups = MemoryMapService.FilterMemoryRegionGroups(groups, PathExcludeExpression, false);
        foreach (var group in groups.OrderBy(i => i.Order))
        {
            displayService.WriteInformation($"- Path: File = '{group.Path}', VSS = {MemoryMapService.FormatSize(group.Vss)}, RSS = {MemoryMapService.FormatSize(group.Rss)}, PSS = {MemoryMapService.FormatSize(group.Pss)}, USS = {MemoryMapService.FormatSize(group.Uss)}");
            if (DisplayRegions)
            {
                var groupRegions = regions.Where(i => i.Path == group.Path).OrderBy(i => i.Start);
                foreach (var region in groupRegions)
                    displayService.WriteInformation($"  - Region: {MemoryMapService.FormatAddress(region.Start)}-{MemoryMapService.FormatAddress(region.End)}, VSS = {MemoryMapService.FormatSize(region.Vss)}, RSS = {MemoryMapService.FormatSize(region.Rss)}, PSS = {MemoryMapService.FormatSize(region.Pss)}, USS = {MemoryMapService.FormatSize(region.Uss)}, Permissions = {MemoryMapService.FormatPermissions(region.Permissions)}");
                var regionTotal = MemoryMapService.GetMemoryRegionsTotal(groupRegions);
                displayService.WriteInformation($"  - Region Total: Count = {regionTotal.Count}, VSS = {MemoryMapService.FormatSize(regionTotal.Vss)}, RSS = {MemoryMapService.FormatSize(regionTotal.Rss)}, PSS = {MemoryMapService.FormatSize(regionTotal.Pss)}, USS = {MemoryMapService.FormatSize(regionTotal.Uss)}");
            }
        }
        var pathTotal = MemoryMapService.GetMemoryRegionGroupsTotal(groups);
        displayService.WriteInformation($"- Path Total: Count = {pathTotal.Count}, VSS = {MemoryMapService.FormatSize(pathTotal.Vss)}, RSS = {MemoryMapService.FormatSize(pathTotal.Rss)}, PSS = {MemoryMapService.FormatSize(pathTotal.Pss)}, USS = {MemoryMapService.FormatSize(pathTotal.Uss)}");
    }
    #endregion

}