using ByteZoo.Blog.Common.ManagedDiagnostics;
using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Heap viewer controller (dumpheap)
/// </summary>
[Verb("ClrMD-Heap", HelpText = "Heap viewer.")]
public class HeapController : DumpController
{

    #region Properties
    /// <summary>
    /// Display statistics only
    /// </summary>
    [Option("statisticsOnly", HelpText = "Display statistics only.")]
    public bool StatisticsOnly { get; set; }

    /// <summary>
    /// Filter heap objects by MethodTable
    /// </summary>
    [Option("methodTable", SetName = "FilterMethodTable", HelpText = "Filter heap objects by MethodTable.")]
    public string? FilterByMethodTable { get; set; }

    /// <summary>
    /// Filter heap objects by type
    /// </summary>
    [Option("type", SetName = "FilterType", HelpText = "Filter heap objects by type.")]
    public string? FilterByType { get; set; }

    /// <summary>
    /// Filter heap objects by free space
    /// </summary>
    [Option("free", SetName = "FilterFree", HelpText = "Filter heap objects by free space.")]
    public bool FilterByFree { get; set; }

    /// <summary>
    /// Filter heap objects by invalid type
    /// </summary>
    [Option("invalid", SetName = "FilterInvalid", HelpText = "Filter heap objects by invalid type.")]
    public bool FilterByInvalid { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        DisplayHeapObjects(GetHeapObjects(runtime));
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return heap objects
    /// </summary>
    /// <param name="runtime"></param>
    /// <returns></returns>
    private IEnumerable<ClrObject> GetHeapObjects(ClrRuntime runtime)
    {
        if (!string.IsNullOrEmpty(FilterByMethodTable))
        {
            var methodTable = ParseAddress(FilterByMethodTable);
            return runtime.Heap.EnumerateObjects().Where(i => i.Type != null && i.Type.MethodTable == methodTable);
        }
        else if (!string.IsNullOrEmpty(FilterByType))
            return runtime.Heap.EnumerateObjects().Where(i => i.Type != null && i.Type.Name != null && i.Type.Name.StartsWith(FilterByType));
        else if (FilterByFree)
            return runtime.Heap.EnumerateObjects().Where(i => i.IsFree);
        else if (FilterByInvalid)
            return runtime.Heap.EnumerateObjects().Where(i => !i.IsValid || i.Type == null);
        else
            return runtime.Heap.EnumerateObjects();
    }

    /// <summary>
    /// Display heap objects
    /// </summary>
    /// <param name="clrObjects"></param>
    private void DisplayHeapObjects(IEnumerable<ClrObject> clrObjects)
    {
        var totalSize = 0UL;
        int totalObjects = 0;
        var statistics = new Dictionary<ulong, TypeStatistics>();
        foreach (var clrObject in clrObjects)
        {
            if (!StatisticsOnly)
                displayService.WriteInformation($"Object: Address = {GetAddress(clrObject.Address)}, MT = {GetAddress(clrObject.Type?.MethodTable)}, Type = {clrObject.Type?.Name}, References = {clrObject.EnumerateReferences().Count()}");
            if (clrObject.Type != null)
            {
                if (!statistics.TryGetValue(clrObject.Type.MethodTable, out var type))
                    statistics.Add(clrObject.Type.MethodTable, type = new TypeStatistics(clrObject.Type.Name ?? "<N/A>", clrObject.Size));
                else
                    type.AddInstance(clrObject.Size);
                totalSize += clrObject.Size;
                totalObjects++;
            }
        }
        foreach (var (methodTable, typeInstance) in statistics)
            displayService.WriteInformation($"Type: MT = {GetAddress(methodTable)}, Count = {typeInstance.Count}, Object Size = {GetSize(typeInstance.ObjectSize)}, Total Size = {GetSize(typeInstance.TotalSize)}, Name = {typeInstance.Name}");
        displayService.WriteInformation($"Total: Objects = {totalObjects}, Total Size = {GetSize(totalSize)}");
    }
    #endregion

}