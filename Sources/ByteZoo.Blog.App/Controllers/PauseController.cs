using CommandLine;

namespace ByteZoo.Blog.App.Controllers;

/// <summary>
/// Pause controller
/// </summary>
[Verb("Pause", HelpText = "Pause operation.")]
public class PauseController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var delay = TimeSpan.FromMilliseconds(200);
        displayService.WriteInformation("Press any key to continue ...");
        if (Console.IsInputRedirected)
            while (Console.Read() == -1)
                Thread.Sleep(delay);
        else
        {
            while (!Console.KeyAvailable)
                Thread.Sleep(delay);
            var key = Console.ReadKey(true);
            if (key.KeyChar == 'e')
                throw new("Pause exception.");
        }
    }
    #endregion

}