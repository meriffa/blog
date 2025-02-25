namespace ByteZoo.Blog.Common.TypeLayout.Models;

/// <summary>
/// Type layout
/// </summary>
/// <param name="type"></param>
/// <param name="size"></param>
/// <param name="overhead"></param>
/// <param name="fields"></param>
/// <param name="paddings"></param>
public readonly struct TypeLayout(Type type, int size, int overhead, FieldLayout[] fields, int paddings) : IEquatable<TypeLayout>
{

    #region Properties
    /// <summary>
    /// Type layout CLR type
    /// </summary>
    public Type Type { get; } = type;

    /// <summary>
    /// Type layout full size (size & overhead)
    /// </summary>
    public readonly int FullSize => Size + Overhead;

    /// <summary>
    /// Type layout size
    /// </summary>
    public int Size { get; } = size;

    /// <summary>
    /// Type layout overhead
    /// </summary>
    public int Overhead { get; } = overhead;

    /// <summary>
    /// Type layout empty space
    /// </summary>
    public int Paddings { get; } = paddings;

    /// <summary>
    /// Type layout fields
    /// </summary>
    public FieldLayout[] Fields { get; } = fields;
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns the hash code for this instance
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode() => (Type, Size, Overhead, Paddings).GetHashCode();

    /// <summary>
    /// Indicates whether this instance and a specified object are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public readonly bool Equals(TypeLayout other) => Type == other.Type && Size == other.Size && Overhead == other.Overhead && Paddings == other.Paddings;

    /// <summary>
    /// Indicates whether this instance and a specified object are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override readonly bool Equals(object? other) => other is TypeLayout layout && Equals(layout);

    /// <summary>
    /// Equals operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(TypeLayout left, TypeLayout right) => left.Equals(right);

    /// <summary>
    /// Not equals operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(TypeLayout left, TypeLayout right) => left != right;
    #endregion

}