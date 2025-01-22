namespace ByteZoo.Blog.Common.Models.ExternalSort;

/// <summary>
/// Split options
/// </summary>
public class OptionsSplit
{

    #region Properties
    /// <summary>
    /// Split options size of unsorted file (bytes)
    /// </summary>
    public int FileSize { get; init; } = 2 * 1024 * 1024;

    /// <summary>
    /// Split options new line separator
    /// </summary>
    public char NewLineSeparator { get; init; } = '\n';
    
    /// <summary>
    /// Split options process handler
    /// </summary>
    public IProgress<double>? ProgressHandler { get; init; }
    #endregion

}