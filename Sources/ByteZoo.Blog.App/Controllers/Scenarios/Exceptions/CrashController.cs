using ByteZoo.Blog.Common.Exceptions;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Scenarios.Exceptions;

/// <summary>
/// Exception (Unhandled Exception) controller
/// </summary>
[Verb("Scenarios-Exceptions-Crash", HelpText = "Exception (Unhandled Exception) operation.")]
public partial class CrashController : Controller
{

    #region Properties
    /// <summary>
    /// Throw unhandled exception or call Thread.Interrupt()
    /// </summary>
    [Option('e', "throwException", Default = false, HelpText = "Throw unhandled exception or call Thread.Interrupt().")]
    public bool ThrowException { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        displayService.WriteInformation("Application crash started.");
        if (ThrowException)
            throw new UnhandledException("Application crash.");
        Thread.CurrentThread.Interrupt();
        displayService.Wait();
    }
    #endregion

}