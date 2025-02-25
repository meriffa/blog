using ByteZoo.Blog.Common.ObjectQuery.Models;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject dictionary extension methods
/// </summary>
public static class ClrObjectDictionaryExtensions
{

    #region Public Methods
    /// <summary>
    /// Return dictionary count
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static int FieldDictionaryCount(this ClrObject clrObject) => clrObject.ReadField<int>("_count");

    /// <summary>
    /// Return dictionary<string, string> entires
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static ObjectInstance FieldDictionaryStringEntriesString(this ClrObject clrObject, int maxLength = 4096)
    {
        var result = new ObjectInstance();
        var entries = GetEntries(clrObject);
        for (int i = 0, length = FieldDictionaryCount(clrObject); i < length; i++)
        {
            var entry = entries.GetStructValue(i);
            result.Add(entry.ReadObjectField("key").AsString(maxLength) ?? "<N/A>", entry.ReadObjectField("value").AsString(maxLength));
        }
        return result;
    }

    /// <summary>
    /// Return dictionary<string, primitive> entires
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static ObjectInstance FieldDictionaryStringEntries<TValue>(this ClrObject clrObject, int maxLength = 4096) where TValue : unmanaged
    {
        var result = new ObjectInstance();
        var entries = GetEntries(clrObject);
        for (int i = 0, length = FieldDictionaryCount(clrObject); i < length; i++)
        {
            var entry = entries.GetStructValue(i);
            result.Add(entry.ReadObjectField("key").AsString(maxLength) ?? "<N/A>", entry.ReadField<TValue>("value"));
        }
        return result;
    }

    /// <summary>
    /// Return dictionary<primitive, string> entires
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="clrObject"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static ObjectInstance FieldDictionaryEntriesString<TKey>(this ClrObject clrObject, int maxLength = 4096) where TKey : unmanaged
    {
        var result = new ObjectInstance();
        var entries = GetEntries(clrObject);
        for (int i = 0, length = FieldDictionaryCount(clrObject); i < length; i++)
        {
            var entry = entries.GetStructValue(i);
            result.Add(entry.ReadField<TKey>("key").ToString() ?? "<N/A>", entry.ReadObjectField("value").AsString(maxLength));
        }
        return result;
    }

    /// <summary>
    /// Return dictionary<primitive, primitive> entires
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static ObjectInstance FieldDictionaryEntries<TKey, TValue>(this ClrObject clrObject) where TKey : unmanaged where TValue : unmanaged
    {
        var result = new ObjectInstance();
        var entries = GetEntries(clrObject);
        for (int i = 0, length = FieldDictionaryCount(clrObject); i < length; i++)
        {
            var entry = entries.GetStructValue(i);
            result.Add(entry.ReadField<TKey>("key").ToString() ?? "<N/A>", entry.ReadField<TValue>("value"));
        }
        return result;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return dictionary entries
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    private static ClrArray GetEntries(ClrObject clrObject) => clrObject.ReadObjectField("_entries").AsArray();
    #endregion

}