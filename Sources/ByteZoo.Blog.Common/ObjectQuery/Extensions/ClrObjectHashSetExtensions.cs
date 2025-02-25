using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject hash set extension methods
/// </summary>
public static class ClrObjectHashSetExtensions
{

    #region Public Methods
    /// <summary>
    /// Return hash set count
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static int FieldHashSetCount(this ClrObject clrObject) => clrObject.ReadField<int>("_count");

    /// <summary>
    /// Return HashSet<string> entires
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string?[] FieldHashSetEntriesString(this ClrObject clrObject, int maxLength = 4096)
    {
        var result = new string?[FieldHashSetCount(clrObject)];
        var entries = GetEntries(clrObject);
        for (int i = 0; i < result.Length; i++)
            result[i] = entries.GetStructValue(i).ReadObjectField("Value").AsString(maxLength);
        return result;
    }

    /// <summary>
    /// Return HashSet<primitive> entires
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static T[] FieldHashSetEntries<T>(this ClrObject clrObject) where T : unmanaged
    {
        var result = new T[FieldHashSetCount(clrObject)];
        var entries = GetEntries(clrObject);
        for (int i = 0; i < result.Length; i++)
            result[i] = entries.GetStructValue(i).ReadField<T>("Value");
        return result;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return hash set entries
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    private static ClrArray GetEntries(ClrObject clrObject) => clrObject.ReadObjectField("_entries").AsArray();
    #endregion

}