using CommandLine;
using System.Net.Http.Json;

namespace ByteZoo.Blog.App.Controllers.Scenarios.Hangs;

/// <summary>
/// Application Hang (web service) controller
/// </summary>
[Verb("Scenarios-Hangs-LockService", HelpText = "Application Hang (web service) operation.")]
public partial class LockServiceController : ServiceController
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        Task.Run(async () =>
        {
            using var client = GetHttpClient();
            var responseTask = client.GetAsync("/Api/Hangs/Lock");
            displayService.WriteInformation("Service request started.");
            using var response = await responseTask;
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<bool>();
            displayService.WriteInformation($"Service request completed (Result = {result}).");
            displayService.Wait();
        }).Wait();
    }
    #endregion

}