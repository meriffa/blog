using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject array extension methods
/// </summary>
public static class ClrObjectArrayExtensions
{

    #region Public Methods
    /// <summary>
    /// Return array length
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static int FieldArrayLength(this ClrObject clrObject) => clrObject.AsArray().Length;

    /// <summary>
    /// Return string array value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="index"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string? FieldArrayString(this ClrObject clrObject, int index, int maxLength = 4096)
    {
        var elements = clrObject.AsArray();
        var value = elements.GetObjectValue(index >= 0 ? index : elements.Length + index);
        return !value.IsNull ? value.AsString(maxLength) : null;
    }

    /// <summary>
    /// Return primitive array value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T FieldArray<T>(this ClrObject clrObject, int index) where T : unmanaged
    {
        var elements = clrObject.AsArray();
        return elements.GetValue<T>(index >= 0 ? index : elements.Length + index);
    }

    /// <summary>
    /// Return object array value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ClrObject FieldArrayObject(this ClrObject clrObject, int index)
    {
        var elements = clrObject.AsArray();
        return elements.GetObjectValue(index >= 0 ? index : elements.Length + index);
    }

    /// <summary>
    /// Return string array elements
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string?[] FieldArrayElementsString(this ClrObject clrObject, int maxLength = 4096)
    {
        var elements = clrObject.AsArray();
        var result = new string?[elements.Length];
        for (int i = 0; i < result.Length; i++)
        {
            var value = elements.GetObjectValue(i);
            result[i] = !value.IsNull ? value.AsString(maxLength) : null;
        }
        return result;
    }

    /// <summary>
    /// Return primitive array elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static T[] FieldArrayElements<T>(this ClrObject clrObject) where T : unmanaged
    {
        var elements = clrObject.AsArray();
        var result = new T[elements.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = elements.GetValue<T>(i);
        return result;
    }
    #endregion

}