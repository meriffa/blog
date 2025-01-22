namespace ByteZoo.Blog.Common.Models.ExternalSort;

/// <summary>
/// Merge options
/// </summary>
public class OptionsMerge
{

    #region Properties
    /// <summary>
    /// Merge options batch size
    /// </summary>
    public int BatchSize { get; init; } = 10;

    /// <summary>
    /// Merge options input StreamReader buffer size (bytes)
    /// </summary>
    public int InputBufferSize { get; init; } = 65536;
    
    /// <summary>
    /// Merge options output StreamWriter buffer size (bytes)
    /// </summary>
    public int OutputBufferSize { get; init; } = 65536;

    /// <summary>
    /// Merge options process handler
    /// </summary>
    public IProgress<double> ProgressHandler { get; init; } = null!;
    #endregion

}