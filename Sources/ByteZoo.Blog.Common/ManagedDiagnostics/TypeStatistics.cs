namespace ByteZoo.Blog.Common.ManagedDiagnostics;

/// <summary>
/// Type statistics
/// </summary>
/// <param name="name"></param>
/// <param name="size"></param>
public class TypeStatistics(string name, ulong size)
{

    #region Properties
    /// <summary>
    /// Type statistics name
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Type statistics count
    /// </summary>
    public ulong Count { get; set; } = 1;

    /// <summary>
    /// Type statistics object size
    /// </summary>
    public ulong ObjectSize { get; init; } = size;

    /// <summary>
    /// Type statistics total size
    /// </summary>
    public ulong TotalSize { get; set; } = size;
    #endregion

    #region Public Methods
    /// <summary>
    /// Add type instance
    /// </summary>
    /// <param name="instanceSize"></param>
    public void AddInstance(ulong instanceSize)
    {
        Count++;
        TotalSize += instanceSize;
    }
    #endregion

}