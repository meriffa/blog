using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject list extension methods
/// </summary>
public static class ClrObjectListExtensions
{

    #region Public Methods
    /// <summary>
    /// Return list length
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static int FieldListLength(this ClrObject clrObject) => clrObject.ReadField<int>("_size");

    /// <summary>
    /// Return string list value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="index"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string? FieldListString(this ClrObject clrObject, int index, int maxLength = 4096) => GetItems(clrObject).GetObjectValue(index >= 0 ? index : FieldListLength(clrObject) + index).AsString(maxLength);

    /// <summary>
    /// Return primitive list value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clrObject"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T FieldList<T>(this ClrObject clrObject, int index) where T : unmanaged => GetItems(clrObject).GetValue<T>(index >= 0 ? index : FieldListLength(clrObject) + index);

    /// <summary>
    /// Return object list value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ClrObject FieldListObject(this ClrObject clrObject, int index) => GetItems(clrObject).GetObjectValue(index >= 0 ? index : FieldListLength(clrObject) + index);

    /// <summary>
    /// Return string list items
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string?[] FieldListItemsString(this ClrObject clrObject, int maxLength = 4096)
    {
        var result = new string?[FieldListLength(clrObject)];
        var items = GetItems(clrObject);
        for (int i = 0; i < result.Length; i++)
        {
            var value = items.GetObjectValue(i);
            result[i] = !value.IsNull ? value.AsString(maxLength) : null;
        }
        return result;
    }

    /// <summary>
    /// Return primitive list items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static T[] FieldListItems<T>(this ClrObject clrObject) where T : unmanaged
    {
        var result = new T[FieldListLength(clrObject)];
        var items = GetItems(clrObject);
        for (int i = 0; i < result.Length; i++)
            result[i] = items.GetValue<T>(i);
        return result;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return list items
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    private static ClrArray GetItems(ClrObject clrObject) => clrObject.ReadObjectField("_items").AsArray();
    #endregion

}