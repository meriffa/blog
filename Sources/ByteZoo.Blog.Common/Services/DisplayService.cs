using Microsoft.Extensions.Logging;

namespace ByteZoo.Blog.Common.Services;

/// <summary>
/// Display service
/// </summary>
/// <param name="logger"></param>
public class DisplayService(ILogger<DisplayService> logger)
{

    #region Constants
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.ffff";
    #endregion

    #region Public Methods
    /// <summary>
    /// Display application title
    /// </summary>
    /// <param name="title"></param>
    public static void WriteTitle(string title)
    {
        var titleEx = $"* {title} *";
        var line = "".PadLeft(titleEx.Length, '*');
        Console.WriteLine(line);
        Console.WriteLine(titleEx);
        Console.WriteLine(line);
        Console.WriteLine();
    }

    /// <summary>
    /// Display information
    /// </summary>
    /// <param name="text"></param>
    public void WriteInformation(string text)
    {
        Console.WriteLine($"[{DateTime.Now.ToString(DATE_FORMAT)}]: {text}");
        logger.LogInformation("{Text}", text);
    }

    /// <summary>
    /// Display warning
    /// </summary>
    /// <param name="text"></param>
    public void WriteWarning(string text)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteInformation($"[WARNING] {text}");
        Console.ForegroundColor = color;
        logger.LogWarning("{Text}", text);
    }

    /// <summary>
    /// Display error
    /// </summary>
    /// <param name="exception"></param>
    public void WriteError(Exception exception)
    {
        WriteError($"[ERROR] {exception.Message}");
        logger.LogError(exception, "{Error}", exception.Message);
    }

    /// <summary>
    /// Display error
    /// </summary>
    /// <param name="error"></param>
    public void WriteError(string error)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        WriteInformation(error);
        Console.ForegroundColor = color;
        logger.LogError("{Error}", error);
    }
    #endregion

}