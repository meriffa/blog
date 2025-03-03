using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// GC roots viewer controller (gcroot, pathto)
/// </summary>
[Verb("ClrMD-GCRoots", HelpText = "GC roots viewer.")]
public class GCRootsController : DumpController
{

    #region Properties
    /// <summary>
    /// Root kind
    /// </summary>
    [Option("rootKind", HelpText = "Root kind.")]
    public ClrRootKind? RootKind { get; set; }

    /// <summary>
    /// Root target
    /// </summary>
    [Option("target", SetName = "Target", HelpText = "Root target.")]
    public string? Target { get; set; }

    /// <summary>
    /// Include root path
    /// </summary>
    [Option("includeRootPath", SetName = "Target", HelpText = "Include root path.")]
    public bool IncludeRootPath { get; set; }

    /// <summary>
    /// Root source
    /// </summary>
    [Option("source", SetName = "Target", HelpText = "Root source.")]
    public string? Source { get; set; }

    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        if (string.IsNullOrEmpty(Target))
            DisplayAllRoots(runtime);
        else if (string.IsNullOrEmpty(Source))
            DisplayTargetRoots(runtime, ParseAddress(Target));
        else
            DisplayTargetRoots(runtime, ParseAddress(Source), ParseAddress(Target));
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display all roots
    /// </summary>
    /// <param name="runtime"></param>
    private void DisplayAllRoots(ClrRuntime runtime)
    {
        foreach (var root in runtime.Heap.EnumerateRoots())
            if (RootKind == null || root.RootKind == RootKind)
                displayService.WriteInformation($"Root: Kind = {root.RootKind}, Address = {GetAddress(root.Address)}, Type = {root.Object.Type?.Name}");
    }

    /// <summary>
    /// Display target roots
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="target"></param>
    private void DisplayTargetRoots(ClrRuntime runtime, ulong target)
    {
        var rootCount = 0;
        var gcRoot = new GCRoot(runtime.Heap, [target]);
        foreach ((var root, var path) in gcRoot.EnumerateRootPaths())
            if (RootKind == null || root.RootKind == RootKind)
            {
                displayService.WriteInformation($"Root: Kind = {root.RootKind}, Address = {GetAddress(root.Address)}, Type = {root.Object.Type?.Name}");
                if (IncludeRootPath && path != null)
                    DisplayTargetRootPath(runtime.Heap, path, 1);
                rootCount++;
            }
        displayService.WriteInformation($"Target: Address = {GetAddress(target)}, Roots = {rootCount}");
    }

    /// <summary>
    /// Display target roots
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private void DisplayTargetRoots(ClrRuntime runtime, ulong source, ulong target)
    {
        var gcRoot = new GCRoot(runtime.Heap, [target]);
        var start = runtime.Heap.GetObject(source);
        if (!start.IsNull && start.IsValid)
        {
            displayService.WriteInformation($"Target: Address = {GetAddress(target)}, Source = {GetAddress(source)}");
            var path = gcRoot.FindPathFrom(start);
            if (path != null)
                DisplayTargetRootPath(runtime.Heap, path, 1);
            else
                displayService.WriteInformation($"Target: Address = {GetAddress(target)}, Source = {GetAddress(source)}, Path Is Empty");
        }
        else
            displayService.WriteInformation($"Target: Address = {GetAddress(target)}, Source = {GetAddress(source)}, No Path Found");
    }

    /// <summary>
    /// Display target root path
    /// </summary>
    /// <param name="heap"></param>
    /// <param name="path"></param>
    /// <param name="level"></param>
    private void DisplayTargetRootPath(ClrHeap heap, GCRoot.ChainLink path, int level)
    {
        var clrType = heap.GetObjectType(path.Object);
        displayService.WriteInformation($"{"-".PadLeft(level)} Path: Address = {GetAddress(path.Object)}, MT = {GetAddress(clrType?.MethodTable)}, Type = {clrType?.Name}");
        if (path.Next != null)
            DisplayTargetRootPath(heap, path.Next, level + 1);
    }
    #endregion

}