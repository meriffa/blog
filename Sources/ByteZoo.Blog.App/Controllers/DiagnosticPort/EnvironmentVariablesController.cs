using CommandLine;

namespace ByteZoo.Blog.App.Controllers.DiagnosticPort;

/// <summary>
/// Environment variables controller
/// </summary>
[Verb("DiagnosticPort-EnvironmentVariables", HelpText = "DiagnosticPort environment variables operation.")]
public class EnvironmentVariablesController : DiagnosticPortController
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