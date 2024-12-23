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
            try
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'e')
                    throw new("Type exception.");
            }
            catch (Exception ex)
            {
                displayService.WriteError(ex);
            }
        }
    }
    #endregion

}