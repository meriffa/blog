using CommandLine;
using Microsoft.Diagnostics.NETCore.Client;

namespace ByteZoo.Blog.App.Controllers.DiagnosticsPort;

/// <summary>
/// DiagnosticsPort controller
/// </summary>
public abstract class DiagnosticsPortController : Controller
{

    #region Properties
    /// <summary>
    /// Target process id
    /// </summary>
    [Option('p', "processId", Required = true, HelpText = "Target process id.")]
    public int ProcessId { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return DiagnosticsClient instance
    /// </summary>
    protected DiagnosticsClient GetDiagnosticsClient() => new(ProcessId);

    /// <summary>
    /// Create wait task
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    protected Task CreateWaitTask(EventPipeSession session) => Task.Run(() =>
    {
        displayService.Wait();
        session.Stop();
    });
    #endregion

}