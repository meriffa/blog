using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// Native heap segments viewer controller (eeheap -loader)
/// </summary>
[Verb("ClrMD-NativeHeapSegments", HelpText = "Native heap segments viewer.")]
public class NativeHeapSegmentsController : DumpController
{

    #region Properties
    /// <summary>
    /// Native segment kind
    /// </summary>
    [Option("segmentKind", HelpText = "Native segment kind.")]
    public NativeHeapKind? SegmentKind { get; set; }

    /// <summary>
    /// Native segment associated GC Heap
    /// </summary>
    [Option("gcHeapIndex", HelpText = "Native segment kind.")]
    public int? GCHeapIndex { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        foreach (var heap in runtime.EnumerateClrNativeHeaps())
            if ((SegmentKind == null || heap.Kind == SegmentKind) && (GCHeapIndex == null || heap.GCHeap == GCHeapIndex))
                displayService.WriteInformation($"Native Heap: Kind = {heap.Kind}, Range = {GetAddress(heap.MemoryRange.Start)}:{GetAddress(heap.MemoryRange.End)} ({GetAddress(heap.MemoryRange.Length)}), State = {heap.State}, GC Heap = {heap.GCHeap}");
    }
    #endregion

}