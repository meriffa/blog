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
    /// Type statistics size
    /// </summary>
    public ulong Size { get; set; } = size;
    #endregion

    #region Public Methods
    /// <summary>
    /// Update type statistics
    /// </summary>
    /// <param name="size"></param>
    public void Update(ulong size)
    {
        Count++;
        Size += size;
    }
    #endregion

}