using CommandLine;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.App.Controllers.ManagedDiagnostics;

/// <summary>
/// Threads viewer controller (clrthreads, clrstack)
/// </summary>
[Verb("ClrMD-Threads", HelpText = "Threads viewer.")]
public class ThreadsController : DumpController
{

    #region Constants
    private const string THREAD_TYPE = "System.Threading.Thread";
    private const string THREAD_ID = "_managedThreadId";
    private const string EXECUTION_CONTEXT_FIELD = "_executionContext";
    #endregion

    #region Properties
    /// <summary>
    /// Filter threads by thread id
    /// </summary>
    [Option("threadId", HelpText = "Filter threads by thread id.")]
    public int? ThreadId { get; set; }

    /// <summary>
    /// Filter stack frame by index
    /// </summary>
    [Option("stackFrameIndex", HelpText = "Filter stack frame by index.")]
    public int? StackFrameIndex { get; set; }

    /// <summary>
    /// Include stack trace
    /// </summary>
    [Option("includeStackTrace", HelpText = "Include stack trace.")]
    public bool IncludeStackTrace { get; set; }
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
            if (ThreadId == null || thread.ManagedThreadId == ThreadId)
            {
                var clrObject = GetThreadObject(runtime, thread.ManagedThreadId);
                var executionContext = GetExecutionContext(clrObject);
                displayService.WriteInformation($"Thread: ID = {thread.ManagedThreadId,4}, OS ID = {thread.OSThreadId:X4}, Address = {GetAddress(thread.Address)}, Live = {thread.IsAlive}, GC = {thread.IsGc}, GC Mode = {thread.GCMode}, State = {GetThreadState(thread.State)}, Finalizer = {thread.IsFinalizer}, Lock Count = {(int)thread.LockCount}, Exception = {thread.CurrentException?.ToString() ?? "<None>"}, Thread Object = {GetAddress(clrObject?.Address)}, Thread Execution Context = {GetAddress(executionContext?.Address)}");
                if (IncludeStackTrace)
                    DisplayStackTrace(thread);
            }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display stack trace
    /// </summary>
    /// <param name="thread"></param>
    private void DisplayStackTrace(ClrThread thread)
    {
        var stackFrameIndex = 0;
        foreach (var frame in thread.EnumerateStackTrace())
        {
            if (StackFrameIndex == null || stackFrameIndex == StackFrameIndex)
                displayService.WriteInformation($"Stack Frame #{stackFrameIndex}: SP = {GetAddress(frame.StackPointer)}, IP = {GetAddress(frame.InstructionPointer)}, Kind = {frame.Kind}, Call Site = {frame.Method?.ToString() ?? frame.ToString()}");
            stackFrameIndex++;
        }
    }

    /// <summary>
    /// Return thread state
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private static string GetThreadState(ClrThreadState state)
    {
        var result = new List<string>();
        if (state.HasFlag(ClrThreadState.TS_AbortRequested))
            result.Add("Abort Requested");
        if (state.HasFlag(ClrThreadState.TS_GCSuspendPending))
            result.Add("Suspend Pending (GC)");
        if (state.HasFlag(ClrThreadState.TS_UserSuspendPending))
            result.Add("Suspend Pending (User)");
        if (state.HasFlag(ClrThreadState.TS_DebugSuspendPending))
            result.Add("Suspend Pending (Debug)");
        if (state.HasFlag(ClrThreadState.TS_Background))
            result.Add("Background");
        if (state.HasFlag(ClrThreadState.TS_Unstarted))
            result.Add("Not Started");
        if (state.HasFlag(ClrThreadState.TS_Dead))
            result.Add("Dead");
        if (state.HasFlag(ClrThreadState.TS_CoInitialized))
            result.Add("CoInitialized");
        if (state.HasFlag(ClrThreadState.TS_InSTA))
            result.Add("STA");
        if (state.HasFlag(ClrThreadState.TS_InMTA))
            result.Add("MTA");
        if (state.HasFlag(ClrThreadState.TS_Aborted))
            result.Add("Aborted");
        if (state.HasFlag(ClrThreadState.TS_TPWorkerThread))
            result.Add("ThreadPool Worker Thread");
        if (state.HasFlag(ClrThreadState.TS_CompletionPortThread))
            result.Add("Completion Port Thread");
        if (state.HasFlag(ClrThreadState.TS_AbortInitiated))
            result.Add("Abort Initiated");
        return result.Count > 0 ? string.Join(", ", result) : "---";
    }

    /// <summary>
    /// Return thread object
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="threadId"></param>
    /// <returns></returns>
    private static ClrObject? GetThreadObject(ClrRuntime runtime, int threadId)
    {
        foreach (var clrObject in runtime.Heap.EnumerateObjects().Where(i => i.Type?.Name == THREAD_TYPE))
            if (clrObject.ReadField<int>(THREAD_ID) == threadId)
                return clrObject;
        return null;
    }

    /// <summary>
    /// Return thread execution context
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    private static ClrObject? GetExecutionContext(ClrObject? clrObject) => clrObject != null && clrObject.Value.ReadObjectField(EXECUTION_CONTEXT_FIELD) is ClrObject field && !field.IsNull ? field : null;
    #endregion

}