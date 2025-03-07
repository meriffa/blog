namespace ByteZoo.Blog.Profiler.Services;

/// <summary>
/// Display service
/// </summary>
public static class DisplayService
{

    #region Public Methods
    /// <summary>
    /// Display information
    /// </summary>
    /// <param name="text"></param>
    public static void WriteInformation(string text) => Console.WriteLine($"[Profiler] {text}");

    /// <summary>
    /// Display error
    /// </summary>
    /// <param name="error"></param>
    public static void WriteError(string error)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        WriteInformation(error);
        Console.ForegroundColor = color;
    }
    #endregion

}