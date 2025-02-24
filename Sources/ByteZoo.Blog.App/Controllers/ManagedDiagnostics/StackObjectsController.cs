using CommandLine;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// Stack objects viewer controller (dumpstackobjects)
/// </summary>
[Verb("ClrMD-StackObjects", HelpText = "Stack objects viewer.")]
public class StackObjectsController : DumpController
{

    #region Properties
    /// <summary>
    /// Filter threads by thread id
    /// </summary>
    [Option("threadId", HelpText = "Filter stack objects by thread id.")]
    public int? ThreadId { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        using var runtime = GetClrRuntime(target);
        foreach (var thread in runtime.Threads)
            if (ThreadId == null || ThreadId == thread.ManagedThreadId)
                foreach (var root in thread.EnumerateStackRoots())
                    displayService.WriteInformation($"Stack Object: Thread ID = {thread.ManagedThreadId}, SP/REG = {GetAddress(root.Address)}, Object = {GetAddress(root.Object.Address)}, Type = {root.Object.Type?.Name}");
    }
    #endregion

    #region Private Methods
    #endregion

}