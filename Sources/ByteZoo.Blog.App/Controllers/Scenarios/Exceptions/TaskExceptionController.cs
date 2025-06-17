using CommandLine;
using Microsoft.Diagnostics.NETCore.Client;
using System.Net;

namespace ByteZoo.Blog.App.Controllers.Scenarios.Exceptions;

/// <summary>
/// Task Exception (UnobservedTaskException) controller
/// </summary>
[Verb("Scenarios-Exceptions-Task", HelpText = "Task Exception (UnobservedTaskException) operation.")]
public partial class TaskExceptionController : ServiceController
{

    #region Constants
    private static readonly string[] REQUEST_TYPE_NAMES = ["Success", "Error", "Hang"];
    private static readonly string[] REQUEST_TYPE_URLS = ["/Api/Weather/GenerateForecast?numberOfDays=5", "/Api/Exceptions/Crash", "/Api/Hangs/Lock"];
    private static readonly double[] REQUEST_PROBABILITIES = [0.5d, 0.3d, 0.2d];
    #endregion

    #region Properties
    /// <summary>
    /// Request count
    /// </summary>
    [Option("requestCount", Required = true, HelpText = "RequestCount.")]
    public int RequestCount { get; set; }

    /// <summary>
    /// Catch task exceptions flag
    /// </summary>
    [Option("catchTaskExceptions", HelpText = "Catch task exceptions flag.")]
    public bool CatchTaskExceptions { get; set; }

    /// <summary>
    /// Capture dump flag
    /// </summary>
    [Option("captureDump", HelpText = "Capture dump flag.")]
    public bool CaptureDump { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var client = GetHttpClient();
        TaskScheduler.UnobservedTaskException += (o, e) => HandleUnobservedTaskException(e);
        AppDomain.CurrentDomain.UnhandledException += (o, e) => displayService.WriteError((Exception)e.ExceptionObject);
        for (int i = 0; i < RequestCount; i++)
        {
            int index = i + 1;
            Task.Factory.StartNew(async () => await SendRequest(client, index));
        }
        if (!CatchTaskExceptions)
            TriggerUnobservedTaskException();
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Send request
    /// </summary>
    /// <param name="client"></param>
    /// <param name="index"></param>
    private async Task SendRequest(HttpClient client, int index)
    {
        try
        {
            var requestType = GetRequestType();
            displayService.WriteInformation($"Task started (Index = {index}, Type = {REQUEST_TYPE_NAMES[requestType]}).");
            var response = await client.GetAsync(REQUEST_TYPE_URLS[requestType]);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new($"Request #{index} failed.");
            displayService.WriteInformation($"Task completed (Index = {index}, Result = {response.StatusCode}).");
        }
        catch (Exception ex)
        {
            if (CatchTaskExceptions)
                displayService.WriteError($"Task failed (Index = {index}, Exception = '{ex.Message}').");
            else
                throw;
        }
    }

    /// <summary>
    /// Return request type
    /// </summary>
    /// <returns></returns>
    private static int GetRequestType()
    {
        var value = Random.Shared.NextDouble();
        var cumulativeProbability = 0.0d;
        for (int i = 0; i < REQUEST_PROBABILITIES.Length; i++)
        {
            cumulativeProbability += REQUEST_PROBABILITIES[i];
            if (value < cumulativeProbability)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Trigger TaskScheduler.UnobservedTaskException
    /// </summary>
    private static void TriggerUnobservedTaskException()
    {
        Thread.Sleep(1000);
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    /// <summary>
    /// Handle TaskScheduler.UnobservedTaskException
    /// </summary>
    /// <param name="e"></param>
    private void HandleUnobservedTaskException(UnobservedTaskExceptionEventArgs e)
    {
        displayService.WriteError(e.Exception);
        if (CaptureDump)
            CaptureCoreDump();
    }

    /// <summary>
    /// Capture core dump
    /// </summary>
    private void CaptureCoreDump()
    {
        var path = Path.Combine(Path.GetTempPath(), $"CoreDump_{DateTime.Now:yyyy-MM-dd}");
        var client = new DiagnosticsClient(Environment.ProcessId);
        client.WriteDump(DumpType.Full, path);
        displayService.WriteWarning($"Core dump file '{path}' created.");
    }
    #endregion

}