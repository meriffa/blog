using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Generate file controller
/// </summary>
[Verb("Tools-FileGenerate", HelpText = "Generate file operation.")]
public class FileGenerateController : Controller
{

    #region Properties
    /// <summary>
    /// Output file
    /// </summary>
    [Option('o', "output", Required = true, HelpText = "Output file.")]
    public string OutputFile { get; set; } = null!;

    /// <summary>
    /// Line format
    /// </summary>
    [Option('f', "format", Required = true, HelpText = "Line format.")]
    public string LineFormat { get; set; } = null!;

    /// <summary>
    /// Number of lines
    /// </summary>
    [Option('l', "lines", Required = true, HelpText = "Number of lines.")]
    public int Lines { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var writer = new StreamWriter(OutputFile, true);
        for (int i = 1; i <= Lines; i++)
            writer.WriteLine(string.Format(LineFormat, i));
        displayService.WriteInformation($"File '{OutputFile}' generated.");
    }
    #endregion

}