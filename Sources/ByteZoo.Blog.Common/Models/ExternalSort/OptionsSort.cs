namespace ByteZoo.Blog.Common.Models.ExternalSort;

/// <summary>
/// Sort options
/// </summary>
public class OptionsSort
{

    #region Properties
    /// <summary>
    /// Sort options comparer
    /// </summary>
    public IComparer<string> Comparer { get; init; } = Comparer<string>.Default;

    /// <summary>
    /// Sort options input StreamReader buffer size (bytes)
    /// </summary>
    public int InputBufferSize { get; init; } = 65536;

    /// <summary>
    /// Sort options output StreamWriter buffer size (bytes)
    /// </summary>
    public int OutputBufferSize { get; init; } = 65536;

    /// <summary>
    /// Sort options process handler
    /// </summary>
    public IProgress<double> ProgressHandler { get; init; } = null!;
    #endregion

}