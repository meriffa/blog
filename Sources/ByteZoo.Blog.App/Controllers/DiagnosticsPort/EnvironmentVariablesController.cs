using CommandLine;

namespace ByteZoo.Blog.App.Controllers.DiagnosticsPort;

/// <summary>
/// Environment variables controller
/// </summary>
[Verb("DiagnosticsPort-EnvironmentVariables", HelpText = "DiagnosticsPort environment variables operation.")]
public class EnvironmentVariablesController : DiagnosticsPortController
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var client = GetDiagnosticsClient();
        foreach (var variable in client.GetProcessEnvironment())
            displayService.WriteInformation($"Environment Variable: Name = {variable.Key}, Value = '{variable.Value}'");
    }
    #endregion

}