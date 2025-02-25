using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject member extension methods
/// </summary>
public static class ClrObjectMemberExtensions
{

    #region Public Methods
    /// <summary>
    /// Return string property value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static string? PropertyString(this ClrObject clrObject, string fieldName) => FieldString(clrObject, $"<{fieldName}>k__BackingField");

    /// <summary>
    /// Return string field value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static string? FieldString(this ClrObject clrObject, string fieldName) => clrObject.ReadStringField(fieldName);

    /// <summary>
    /// Return primitive property value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static T Property<T>(this ClrObject clrObject, string fieldName) where T : unmanaged => Field<T>(clrObject, $"<{fieldName}>k__BackingField");

    /// <summary>
    /// Return primitive field value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static T Field<T>(this ClrObject clrObject, string fieldName) where T : unmanaged => clrObject.ReadField<T>(fieldName);

    /// <summary>
    /// Return object property value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static ClrObject PropertyObject(this ClrObject clrObject, string fieldName) => FieldObject(clrObject, $"<{fieldName}>k__BackingField");

    /// <summary>
    /// Return object field value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static ClrObject FieldObject(this ClrObject clrObject, string fieldName) => clrObject.ReadObjectField(fieldName);
    #endregion

}