using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Type controller
/// </summary>
[Verb("Concepts-Type", HelpText = "Type operation.")]
public class TypeController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute() => displayService.Wait();
    #endregion

}