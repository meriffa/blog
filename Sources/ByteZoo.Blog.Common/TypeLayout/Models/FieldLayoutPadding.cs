namespace ByteZoo.Blog.Common.TypeLayout.Models;

/// <summary>
/// Padding field layout (between fields)
/// </summary>
/// <param name="offset"></param>
/// <param name="size"></param>
public sealed class FieldLayoutPadding(int offset, int size) : FieldLayout(offset, size)
{

    #region Properties
    /// <summary>
    /// Field name
    /// </summary>
    public override string Name => "<Padding>";
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns the hash code for this instance
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => (Offset, Size).GetHashCode();

    /// <summary>
    /// Indicates whether this instance and a specified object are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(object? other) => other is FieldLayoutPadding padding && Offset == padding.Offset && Size == padding.Size;
    #endregion

}