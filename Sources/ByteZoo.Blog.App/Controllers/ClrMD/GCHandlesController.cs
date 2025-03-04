using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// GC handles viewer controller
/// </summary>
[Verb("ClrMD-GCHandles", HelpText = "GC handles viewer.")]
public class GCHandlesController : DumpController
{

    #region Properties
    /// <summary>
    /// GC handle kind
    /// </summary>
    [Option("handleKind", HelpText = "GC handle kind.")]
    public ClrHandleKind? HandleKind { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        foreach (var handle in runtime.EnumerateHandles())
            if (HandleKind == null || handle.HandleKind == HandleKind)
                displayService.WriteInformation($"GC Handle: Kind = {handle.HandleKind}, Root = {handle.RootKind}, Pinned = {handle.IsPinned}, Strong = {handle.IsStrong}, Interior = {handle.IsInterior}, Reference Count = {handle.ReferenceCount}, Address = {GetAddress(handle.Address)}, Object Type = {handle.Object.Type?.Name ?? "<N/A>"}, Dependent Type = {handle.Dependent.Type?.Name ?? "<N/A>"}");
    }
    #endregion

}