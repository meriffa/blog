using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// GC roots viewer controller (gcroot)
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
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        foreach (var root in runtime.Heap.EnumerateRoots())
            if (RootKind == null || root.RootKind == RootKind)
                displayService.WriteInformation($"Root: Kind = {root.RootKind}, Address = {GetAddress(root.Address)}, Type = {root.Object.Type?.Name}");
    }
    #endregion

}