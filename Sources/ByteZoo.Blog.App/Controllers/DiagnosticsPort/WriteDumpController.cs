using CommandLine;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using System.Diagnostics.Tracing;

namespace ByteZoo.Blog.App.Controllers.DiagnosticsPort;

/// <summary>
/// Write dump controller
/// </summary>
[Verb("DiagnosticsPort-WriteDump", HelpText = "DiagnosticsPort write dump operation.")]
public class WriteDumpController : DiagnosticsPortController
{

    #region Private Members
    private int dumpEnabled = 1;
    private int dumpSuffix = 1;
    #endregion

    #region Properties
    /// <summary>
    /// Dump path
    /// </summary>
    [Option("path", Required = true, HelpText = "Dump path.")]
    public string Path { get; set; } = null!;

    /// <summary>
    /// Dump type
    /// </summary>
    [Option("type", Required = false, Default = DumpType.Full, HelpText = "Dump type.")]
    public DumpType Type { get; set; }

    /// <summary>
    /// CPU threshold
    /// </summary>
    [Option("threshold", HelpText = "CPU threshold.")]
    public double? Threshold { get; set; }

    /// <summary>
    /// Pause between dumps
    /// </summary>
    [Option("pause", Default = 5, HelpText = "Pause between dumps.")]
    public int Pause { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var client = GetDiagnosticsClient();
        if (Threshold != null)
        {
            var tasks = new Task[2];
            var provider = new EventPipeProvider("System.Runtime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.None, new Dictionary<string, string> { ["EventCounterIntervalSec"] = "1" });
            using var session = client.StartEventPipeSession(provider);
            tasks[0] = CreateCpuUsageTask(client, session);
            tasks[1] = CreateWaitTask(session);
            Task.WaitAny(tasks);
        }
        else
            WriteDump(client);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Write dump file
    /// </summary>
    /// <param name="client"></param>
    /// <param name="suffix"></param>
    private void WriteDump(DiagnosticsClient client, int? suffix = null)
    {
        var path = suffix == null ? Path : $"{Path}.{suffix}";
        client.WriteDump(Type, path);
        displayService.WriteInformation($"Dump file '{path}' created.");
    }

    /// <summary>
    /// Create CPU usage task
    /// </summary>
    /// <param name="client"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    private Task CreateCpuUsageTask(DiagnosticsClient client, EventPipeSession session) => Task.Run(() =>
    {
        using var source = new EventPipeEventSource(session.EventStream);
        source.Dynamic.All += e =>
        {
            if (e.EventName.Equals("EventCounters"))
            {
                var payloadVal = (IDictionary<string, object>)e.PayloadValue(0);
                var payloadFields = (IDictionary<string, object>)payloadVal["Payload"];
                if (payloadFields["Name"].ToString()!.Equals("cpu-usage") && double.Parse(payloadFields["Mean"].ToString()!) > Threshold)
                    if (Interlocked.Exchange(ref dumpEnabled, 0) == 1)
                    {
                        WriteDump(client, dumpSuffix++);
                        Task.Run(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(Pause));
                            dumpEnabled = 1;
                        });
                    }
            }
        };
        source.Process();
    });
    #endregion

}