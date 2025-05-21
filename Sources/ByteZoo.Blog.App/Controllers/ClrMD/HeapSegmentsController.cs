using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Heap segments viewer controller (eeheap -gc)
/// </summary>
[Verb("ClrMD-HeapSegments", HelpText = "Heap segments viewer.")]
public class HeapSegmentsController : DumpController
{

    #region Properties
    /// <summary>
    /// GC segment kind
    /// </summary>
    [Option("segmentKind", HelpText = "GC segment kind.")]
    public GCSegmentKind? SegmentKind { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        foreach (var heap in runtime.Heap.SubHeaps)
            DisplayHeapSegments(heap);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display heap segments
    /// </summary>
    /// <param name="heap"></param>
    private void DisplayHeapSegments(ClrSubHeap heap)
    {
        displayService.WriteInformation($"GC Heap: Index = {heap.Index}, Segments = {heap.Segments.Length}");
        foreach (var segment in heap.Segments.OrderBy(i => i.Address))
            if (SegmentKind == null || segment.Kind == SegmentKind)
                // [Committed...[Allocated (Used / Filled)]...][Reserved]
                displayService.WriteInformation($"Segment: Type = {GetSegmentType(segment)}, Address = {GetAddress(segment.Address)}, Committed = {GetAddress(segment.CommittedMemory.Start)}-{GetAddress(segment.CommittedMemory.End)} ({GetAddress(segment.CommittedMemory.Length)}), Allocated = {GetAddress(segment.Start)}-{GetAddress(segment.End)} ({GetAddress(segment.Length)}), Reserved = {GetAddress(segment.ReservedMemory.Start)}-{GetAddress(segment.ReservedMemory.End)} ({GetAddress(segment.ReservedMemory.Length)})");
    }

    /// <summary>
    /// Return segment type
    /// </summary>
    /// <param name="segment"></param>
    /// <returns></returns>
    private static string GetSegmentType(ClrSegment segment) => segment.Kind switch
    {
        GCSegmentKind.Generation0 => "SOH (Gen0)",
        GCSegmentKind.Generation1 => "SOH (Gen1)",
        GCSegmentKind.Generation2 => "SOH (Gen2)",
        GCSegmentKind.Large => "LOH",
        GCSegmentKind.Pinned => "POH",
        GCSegmentKind.Frozen => "FOH",
        GCSegmentKind.Ephemeral => "SOH",
        _ => "---"
    };
    #endregion

}