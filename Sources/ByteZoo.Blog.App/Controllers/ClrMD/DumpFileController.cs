using CommandLine;

namespace ByteZoo.Blog.App.Controllers.ClrMD;

/// <summary>
/// Dump file viewer controller
/// </summary>
[Verb("ClrMD-DumpFile", HelpText = "Dump file viewer.")]
public class DumpFileController : DumpController
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var target = GetDataTarget();
        foreach (var info in target.ClrVersions)
        {
            displayService.WriteInformation($"CLR: Flavor = {info.Flavor}, Version = {info.Version}");
            foreach (var library in info.DebuggingLibraries)
                displayService.WriteInformation($"Library: Type = {library.Kind.ToString().ToUpper()}, Platform = {library.Platform}, Architecture = {library.TargetArchitecture}, File = '{library.FileName}'");
        }
    }
    #endregion

}