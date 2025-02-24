using CommandLine;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Tracing;

namespace ByteZoo.Blog.App.Controllers.DiagnosticPort;

/// <summary>
/// Event listener controller
/// </summary>
[Verb("DiagnosticPort-EventListener", HelpText = "DiagnosticPort event listener operation.")]
public class EventListenerController : DiagnosticPortController
{

    #region Properties
    /// <summary>
    /// Event provider (https://learn.microsoft.com/en-us/dotnet/core/diagnostics/well-known-event-providers)
    /// </summary>
    [Option("provider", Required = true, HelpText = "Event provider.")]
    public string Provider { get; set; } = null!;

    /// <summary>
    /// Event level
    /// </summary>
    [Option("level", Required = false, Default = EventLevel.Informational, HelpText = "Event level.")]
    public EventLevel Level { get; set; }

    /// <summary>
    /// Event keywords
    /// </summary>
    [Option("keywords", Required = false, Default = ClrTraceEventParser.Keywords.Default, HelpText = "Event keywords.")]
    public ClrTraceEventParser.Keywords Keywords { get; set; }

    /// <summary>
    /// Event file
    /// </summary>
    [Option("file", HelpText = "Event file.")]
    public string? File { get; set; }

    /// <summary>
    /// Event capture duration [sec]
    /// </summary>
    [Option("duration", HelpText = "Event capture duration [sec].")]
    public int? Duration { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var client = GetDiagnosticsClient();
        var provider = new EventPipeProvider(Provider, Level, (long)Keywords);
        using var session = client.StartEventPipeSession(provider, false);
        var tasks = new List<Task>();
        if (File == null)
            tasks.Add(CreateConsoleOutputTask(session));
        else
            tasks.Add(CreateFileOutputTask(session, File));
        if (Duration != null)
            tasks.Add(CreateDurationTask(Duration.Value));
        tasks.Add(CreateWaitTask(session));
        Task.WaitAny([.. tasks]);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Create console output task
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    private Task CreateConsoleOutputTask(EventPipeSession session) => Task.Run(() =>
    {
        using var source = new EventPipeEventSource(session.EventStream);
        source.Clr.All += e => displayService.WriteInformation(e.ToString());
        source.Process();
    });

    /// <summary>
    /// Create file output task
    /// </summary>
    /// <param name="session"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    private static Task CreateFileOutputTask(EventPipeSession session, string file) => Task.Run(async () =>
    {
        using var stream = new FileStream(file, FileMode.Create, FileAccess.Write);
        await session.EventStream.CopyToAsync(stream);
    });

    /// <summary>
    /// Create duration task
    /// </summary>
    /// <returns></returns>
    private static Task CreateDurationTask(int duration) => Task.Delay(TimeSpan.FromSeconds(duration));
    #endregion

}