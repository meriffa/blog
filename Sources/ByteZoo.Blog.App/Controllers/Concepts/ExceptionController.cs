using ByteZoo.Blog.Common.Exceptions;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Exception controller
/// </summary>
[Verb("Concepts-Exception", HelpText = "Exception operation.")]
public class ExceptionController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        RaiseCurrentThreadException("Application exception instance #1.");
        RaiseCurrentThreadException("Application exception instance #2.");
        RaiseNewThreadException("Application exception instance #3.");
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Raise exception on the current thread
    /// </summary>
    /// <param name="message"></param>
    private void RaiseCurrentThreadException(string message)
    {
        try
        {
            throw new UnhandledException(message);
        }
        catch (Exception ex)
        {
            displayService.WriteError(ex);
        }
    }

    /// <summary>
    /// Raise exception on a new thread
    /// </summary>
    /// <param name="message"></param>
    private void RaiseNewThreadException(string message)
    {
        var t = Task.Factory.StartNew((data) =>
        {
            displayService.WriteInformation($"Task started (Thread ID = {Environment.CurrentManagedThreadId}, Data = '{data}').");
            throw new UnhandledException(data!.ToString()!);
        }, message);
    }
    #endregion

}