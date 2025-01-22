namespace ByteZoo.Blog.Common.Models.ExternalSort;

/// <summary>
/// Options
/// </summary>
public class Options
{

    #region Properties
    /// <summary>
    /// File location
    /// </summary>
    public string FileLocation { get; init; } = Path.GetTempPath();

    /// <summary>
    /// Split options
    /// </summary>
    public OptionsSplit Split { get; init; } = new();

    /// <summary>
    /// Sort options
    /// </summary>
    public OptionsSort Sort { get; init; } = new();

    /// <summary>
    /// Merge options
    /// </summary>
    public OptionsMerge Merge { get; init; } = new();
    #endregion

}