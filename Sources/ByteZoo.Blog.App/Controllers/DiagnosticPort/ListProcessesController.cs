using CommandLine;
using Microsoft.Diagnostics.NETCore.Client;
using System.Diagnostics;
using System.Reflection;

namespace ByteZoo.Blog.App.Controllers.DiagnosticPort;

/// <summary>
/// List processes controller
/// </summary>
[Verb("DiagnosticPort-ListProcesses", HelpText = "DiagnosticPort list processes operation.")]
public class ListProcessesController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        foreach (var process in DiagnosticsClient.GetPublishedProcesses().Select(i => Process.GetProcessById(i)))
            displayService.WriteInformation($"Process: ID = {process.Id}, Name = '{process.ProcessName}', Command Line = '{GetProcessCommandLine(process.Id)}'");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return process command line
    /// </summary>
    /// <param name="processId"></param>
    /// <returns></returns>
    private static string GetProcessCommandLine(int processId)
    {
        var client = new DiagnosticsClient(processId);
        var processInfo = typeof(DiagnosticsClient).GetMethod("GetProcessInfo", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(client, null)!;
        return (string)processInfo.GetType().GetProperty("CommandLine")!.GetValue(processInfo)!;
    }
    #endregion

}