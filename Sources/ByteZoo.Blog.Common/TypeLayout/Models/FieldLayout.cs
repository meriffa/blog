namespace ByteZoo.Blog.Common.TypeLayout.Models;

/// <summary>
/// Field layout
/// </summary>
/// <param name="offset"></param>
/// <param name="size"></param>
public abstract class FieldLayout(int offset, int size)
{

    #region Properties
    /// <summary>
    /// Field size
    /// </summary>
    public int Size { get; } = size;

    /// <summary>
    /// Field offset
    /// </summary>
    public int Offset { get; } = offset;

    /// <summary>
    /// Field name
    /// </summary>
    public abstract string Name { get; }
    #endregion

}