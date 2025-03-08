using ClrDebug;
using CommandLine;
using System.Runtime.InteropServices;

namespace ByteZoo.Blog.App.Controllers.Debugger;

/// <summary>
/// Debugger controller
/// </summary>
[Verb("Debugger-Start", HelpText = "Debugger start operation.")]
public class DebuggerController : Controller
{

    #region Properties
    /// <summary>
    /// DbgShim path
    /// </summary>
    [Option('d', "dbgShimPath", Required = true, HelpText = "DbgShim path.")]
    public string DbgShimPath { get; set; } = null!;

    /// <summary>
    /// Target command line
    /// </summary>
    [Option('c', "commandLine", Required = true, HelpText = "Target command line.")]
    public string TargetCommandLine { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        displayService.WriteInformation("Debugger started.");
        var dbgShim = new DbgShim(NativeLibrary.Load(DbgShimPath));
        var process = dbgShim.CreateProcessForLaunch(TargetCommandLine, true);
        try
        {
            StartDebugger(dbgShim, process.ProcessId, process.ResumeHandle);
        }
        finally
        {
            dbgShim.CloseResumeHandle(process.ResumeHandle);
        }
        displayService.WriteInformation("Debugger completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start debugger
    /// </summary>
    /// <param name="dbgShim"></param>
    /// <param name="processId"></param>
    /// <param name="resumeHandle"></param>
    private void StartDebugger(DbgShim dbgShim, int processId, IntPtr resumeHandle)
    {
        IntPtr unregisterToken = IntPtr.Zero;
        CorDebug? corDebug = null;
        HRESULT result = HRESULT.E_FAIL;
        var wait = new AutoResetEvent(false);
        try
        {
            dbgShim.ResumeProcess(resumeHandle);
            unregisterToken = dbgShim.RegisterForRuntimeStartup(processId, (pCorDebug, _, callbackResult) =>
            {
                corDebug = pCorDebug;
                result = callbackResult;
                wait.Set();
            });
            wait.WaitOne();
        }
        finally
        {
            if (unregisterToken != IntPtr.Zero)
                dbgShim.UnregisterForRuntimeStartup(unregisterToken);
        }
        if (corDebug == null)
            throw new DebugException(result);
        Initialize(corDebug, processId);
        displayService.Wait();
    }

    /// <summary>
    /// Initialize ICorDebug instance
    /// </summary>
    /// <param name="corDebug"></param>
    /// <param name="processId"></param>
    private void Initialize(CorDebug corDebug, int processId)
    {
        corDebug.Initialize();
        var eventHandler = new CorDebugManagedCallback();
        eventHandler.OnAnyEvent += (_, e) =>
        {
            if (e.Kind != CorDebugManagedCallbackKind.ExitProcess)
                e.Controller.Continue(false);
        };
        eventHandler.OnLoadModule += (_, e) => displayService.WriteInformation($"Module loaded (Name = '{e.Module.Name}').");
        eventHandler.OnExitProcess += (_, e) => displayService.WriteInformation($"Process exited (Process ID = {e.Process.Id})");
        corDebug.SetManagedHandler(eventHandler);
        corDebug.DebugActiveProcess(processId, false);
    }
    #endregion

}