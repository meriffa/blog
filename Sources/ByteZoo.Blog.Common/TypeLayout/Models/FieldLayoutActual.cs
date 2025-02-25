using System.Reflection;

namespace ByteZoo.Blog.Common.TypeLayout.Models;

/// <summary>
/// Actual field layout
/// </summary>
/// <param name="offset"></param>
/// <param name="fieldInfo"></param>
/// <param name="size"></param>
public sealed class FieldLayoutActual(int offset, FieldInfo fieldInfo, int size) : FieldLayout(offset, size)
{

    #region Properties
    /// <summary>
    /// Field name
    /// </summary>
    public override string Name => $"{FieldInfo.FieldType.FullName} {FieldInfo.Name}";

    /// <summary>
    /// Field information
    /// </summary>
    public FieldInfo FieldInfo { get; } = fieldInfo;
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns the hash code for this instance
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => (Offset, Size, FieldInfo).GetHashCode();

    /// <summary>
    /// Indicates whether this instance and a specified object are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(object? other) => other is FieldLayoutActual fieldLayout && Offset == fieldLayout.Offset && Size == fieldLayout.Size && FieldInfo == fieldLayout.FieldInfo;
    #endregion

}