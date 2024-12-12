using ByteZoo.Blog.Common.Exceptions;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers;

/// <summary>
/// Exception controller
/// </summary>
[Verb("Exception", HelpText = "Exception operation.")]
public class ExceptionController : Controller
{

    #region Properties
    /// <summary>
    /// Exception message
    /// </summary>
    [Option('m', "message", Required = true, HelpText = "Exception message.")]
    public string Message { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute() => throw new UnhandledException(Message);
    #endregion

}