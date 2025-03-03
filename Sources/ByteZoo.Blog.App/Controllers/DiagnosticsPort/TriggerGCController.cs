using CommandLine;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Tracing;

namespace ByteZoo.Blog.App.Controllers.DiagnosticsPort;

/// <summary>
/// Trigger GC controller
/// </summary>
[Verb("Tools-TriggerGC", HelpText = "Trigger GC operation.")]
public partial class TriggerGCController : DiagnosticsPortController
{

    #region Constants
    private const string PROVIDER_NAME = "Microsoft-Windows-DotNETRuntime";
    #endregion

    #region Properties
    /// <summary>
    /// Start timeout [sec]
    /// </summary>
    [Option("startTimeout", Default = 10, HelpText = "Start timeout [sec].")]
    public int StartTimeout { get; set; }

    /// <summary>
    /// Complete timeout [sec]
    /// </summary>
    [Option("completeTimeout", Default = 30, HelpText = "Complete timeout [sec].")]
    public int CompleteTimeout { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var client = GetDiagnosticsClient();
        var providerMonitoring = new EventPipeProvider(PROVIDER_NAME, EventLevel.Informational, (long)ClrTraceEventParser.Keywords.GC);
        using var sessionMonitoring = client.StartEventPipeSession(providerMonitoring, false);
        var providerTrigger = new EventPipeProvider(PROVIDER_NAME, EventLevel.Informational, (long)ClrTraceEventParser.Keywords.GCHeapCollect);
        using var sessionTrigger = client.StartEventPipeSession(providerTrigger, false);
        using var monitoringInitialized = new ManualResetEventSlim();
        using var collectCompleted = new ManualResetEventSlim();
        var tasks = new List<Task>
        {
            CreateMonitoringTask(sessionMonitoring, monitoringInitialized, collectCompleted),
            CreateTriggerTask(sessionTrigger, monitoringInitialized),
            CreateDurationTask(collectCompleted)
        };
        Task.WaitAny([.. tasks]);
        sessionTrigger.Stop();
        sessionMonitoring.Stop();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Create monitoring task
    /// </summary>
    /// <param name="session"></param>
    /// <param name="monitoringInitialized"></param>
    /// <param name="collectCompleted"></param>
    /// <returns></returns>
    private Task CreateMonitoringTask(EventPipeSession session, ManualResetEventSlim monitoringInitialized, ManualResetEventSlim collectCompleted) => Task.Run(() =>
    {
        using var source = new EventPipeEventSource(session.EventStream);
        source.Clr.GCStart += e => displayService.WriteInformation($"GC Started (Reason = {e.Reason}, Sequence ID = {e.Count}).");
        source.Clr.GCStop += e =>
        {
            displayService.WriteInformation($"GC Stopped (Sequence ID = {e.Count}).");
            collectCompleted.Set();
        };
        monitoringInitialized.Set();
        source.Process();
    });

    /// <summary>
    /// Create trigger task
    /// </summary>
    /// <param name="session"></param>
    /// <param name="monitoringInitialized"></param>
    /// <returns></returns>
    private Task CreateTriggerTask(EventPipeSession session, ManualResetEventSlim monitoringInitialized) => Task.Run(() =>
    {
        if (monitoringInitialized.Wait(TimeSpan.FromSeconds(StartTimeout)))
        {
            displayService.WriteInformation("GC Trigger.");
            using var source = new EventPipeEventSource(session.EventStream);
            source.Process();
        }
        else
            displayService.WriteWarning($"GC monitoring did not start within {StartTimeout} seconds.");
    });

    /// <summary>
    /// Create duration task
    /// </summary>
    /// <param name="collectCompleted"></param>
    /// <returns></returns>
    private Task CreateDurationTask(ManualResetEventSlim collectCompleted) => Task.Run(() =>
    {
        if (!collectCompleted.Wait(TimeSpan.FromSeconds(CompleteTimeout)))
            displayService.WriteWarning($"GC collect did not complete within {CompleteTimeout} seconds.");
    });
    #endregion

}