using ByteZoo.Blog.Common.ManagedDiagnostics;
using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Heap compare controller
/// </summary>
[Verb("ClrMD-HeapCompare", HelpText = "Heap compare.")]
public class HeapCompareController : DumpController
{

    #region Properties
    /// <summary>
    /// Dump compare file
    /// </summary>
    [Option("compareFile", Required = true, HelpText = "Dump compare file.")]
    public string CompareFile { get; set; } = null!;

    /// <summary>
    /// Total size minimum
    /// </summary>
    [Option("totalSizeMinimum", Default = 1UL, HelpText = "Total size minimum.")]
    public ulong TotalSizeMinimum { get; set; }

    /// <summary>
    /// Total size change [%]
    /// </summary>
    [Option("totalSizeIncrease", Default = 5.0d, HelpText = "Total size increase [%].")]
    public double TotalSizeChange { get; set; }

    /// <summary>
    /// Exclude free blocks
    /// </summary>
    [Option("excludeFree", HelpText = "Exclude free blocks.")]
    public bool ExcludeFree { get; set; }

    /// <summary>
    /// Include deleted objects
    /// </summary>
    [Option("includeDeleted", HelpText = "Include deleted objects.")]
    public bool IncludeDeleted { get; set; }

    /// <summary>
    /// Include new objects
    /// </summary>
    [Option("includeNew", HelpText = "Include new objects.")]
    public bool IncludeNew { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        var sourceStatistics = GetHeapObjectTypeStatistics(runtime);
        using var targetCompare = GetDataTarget(CompareFile);
        using var runtimeCompare = GetClrRuntime(targetCompare);
        var targetStatistics = GetHeapObjectTypeStatistics(runtimeCompare);
        var statistics = CompareHeapObjectTypeStatistics(sourceStatistics, targetStatistics);
        DisplayHeapObjectTypeStatistics(statistics);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return heap object type statistics
    /// </summary>
    /// <param name="runtime"></param>
    /// <returns></returns>
    private Dictionary<ulong, TypeStatistics> GetHeapObjectTypeStatistics(ClrRuntime runtime)
    {
        var result = new Dictionary<ulong, TypeStatistics>();
        foreach (var clrObject in runtime.Heap.EnumerateObjects())
            if (!clrObject.IsFree || !ExcludeFree)
            {
                var typeMethodTable = clrObject.Type?.MethodTable ?? ulong.MaxValue;
                if (!result.TryGetValue(typeMethodTable, out var type))
                    result.Add(typeMethodTable, new TypeStatistics(clrObject.Type?.Name ?? "Invalid", clrObject.Size));
                else
                    type.AddInstance(clrObject.Size);
            }
        return result;
    }

    /// <summary>
    /// Compare heap object type statistics
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private Dictionary<ulong, TypeStatisticsCompare> CompareHeapObjectTypeStatistics(Dictionary<ulong, TypeStatistics> source, Dictionary<ulong, TypeStatistics> target)
    {
        var result = new Dictionary<ulong, TypeStatisticsCompare>();
        foreach (var (methodTable, typeInstance) in source)
            if (target.TryGetValue(methodTable, out var targetTypeStatistics) && IncludeTypeStatistics(typeInstance, targetTypeStatistics))
                result.Add(methodTable, new TypeStatisticsCompare(typeInstance, targetTypeStatistics));
            else if (IncludeDeleted && IncludeTypeStatistics(typeInstance))
                result.Add(methodTable, new TypeStatisticsCompare(typeInstance, null));
        if (IncludeNew)
            foreach (var (methodTable, typeInstance) in target)
                if (!source.ContainsKey(methodTable) && IncludeTypeStatistics(typeInstance))
                    result.Add(methodTable, new TypeStatisticsCompare(null, typeInstance));
        return result;
    }

    /// <summary>
    /// Check whether to include object type statistics
    /// </summary>
    /// <param name="sourceTypeStatistics"></param>
    /// <param name="targetTypeStatistics"></param>
    /// <returns></returns>
    private bool IncludeTypeStatistics(TypeStatistics sourceTypeStatistics, TypeStatistics targetTypeStatistics) => targetTypeStatistics.TotalSize >= TotalSizeMinimum && Math.Abs(100.0d * targetTypeStatistics.TotalSize / sourceTypeStatistics.TotalSize - 100.0d) >= TotalSizeChange;

    /// <summary>
    /// Check whether to include object type statistics
    /// </summary>
    /// <param name="typeStatistics"></param>
    /// <returns></returns>
    private bool IncludeTypeStatistics(TypeStatistics typeStatistics) => typeStatistics.TotalSize >= TotalSizeMinimum;

    /// <summary>
    /// Display heap object type statistics
    /// </summary>
    /// <param name="statistics"></param>
    private void DisplayHeapObjectTypeStatistics(Dictionary<ulong, TypeStatisticsCompare> statistics)
    {
        foreach (var (methodTable, typeInstance) in statistics)
            displayService.WriteInformation($"Type: MT = {GetAddress(methodTable)}, Count [{typeInstance.Source?.Count}, {typeInstance.Target?.Count}], Total Size = [{typeInstance.Source?.TotalSize}, {typeInstance.Target?.TotalSize}], Name = {typeInstance.Source?.Name ?? typeInstance.Target?.Name}");
    }
    #endregion

}