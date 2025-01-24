using BenchmarkDotNet.Running;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Benchmark controller
/// </summary>
[Verb("Tools-Benchmark", HelpText = "Benchmark operation.")]
public class BenchmarkController : Controller
{

    #region Properties
    /// <summary>
    /// Benchmark switcher arguments
    /// </summary>
    [Option('a', "args", Required = true, HelpText = "Benchmark switcher arguments.")]
    public string Arguments { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute() => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(Arguments.TrimStart('"').TrimEnd('"').Split(' '));
    #endregion

}