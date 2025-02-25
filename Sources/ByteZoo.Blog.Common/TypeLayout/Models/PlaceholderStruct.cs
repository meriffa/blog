namespace ByteZoo.Blog.Common.TypeLayout.Models;

/// <summary>
/// Placeholder structure used for struct size calculation
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Both fields must have the same type, because the CLR can rearrange the struct.
/// </remarks>
public struct PlaceholderStruct<T>
{

    #region Public Members
    public T PlaceholderField;
    public T OffsetField;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="placeholderField"></param>
    /// <param name="offsetField"></param>
    public PlaceholderStruct(T placeholderField, T offsetField) => (PlaceholderField, OffsetField) = (placeholderField, offsetField);
    #endregion

}