using ByteZoo.Blog.Common.ObjectQuery.Models;
using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject HTTP extension methods
/// </summary>
public static class ClrObjectHttpExtensions
{

    #region Public Methods
    /// <summary>
    /// Return Uri value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string? FieldUri(this ClrObject clrObject, string fieldName, int maxLength = 4096) => clrObject.ReadObjectField(fieldName).ReadObjectField("_string").AsString(maxLength);

    /// <summary>
    /// Return HttpMethod value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string? FieldHttpMethod(this ClrObject clrObject, string fieldName, int maxLength = 4096) => clrObject.ReadObjectField(fieldName).ReadObjectField("_method").AsString(maxLength);

    /// <summary>
    /// Return HttpContext features
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static ObjectInstance? HttpContextFeatures(this ClrObject clrObject)
    {
        if (clrObject.Type?.Fields.First(i => i.Name == "_features") is ClrInstanceField featuresField)
        {
            var features = featuresField.ReadStruct(clrObject.Address, false);
            if (features.Type?.Fields.First(i => i.Name == "<Collection>k__BackingField") is ClrInstanceField collectionField)
            {
                var collection = collectionField.ReadObject(features.Address, true);
                if (!collection.IsNull)
                    return GetHttpContextFeatures(collection);
            }
        }
        return null;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return HttpContext features
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    private static ObjectInstance GetHttpContextFeatures(ClrObject clrObject) => new()
    {
        { "RequestId", clrObject.ReadStringField("_requestId") },
        { "Scheme", clrObject.ReadStringField("_scheme") },
        { "Method", clrObject.ReadStringField("_methodText") },
        { "Path", clrObject.ReadStringField("<Path>k__BackingField") },
        { "QueryString", clrObject.ReadStringField("<QueryString>k__BackingField") },
        { "StatusCode", clrObject.ReadField<int>("_statusCode") },
        { "ConnectionAborted", clrObject.ReadField<bool>("_connectionAborted") },
        { "KeepAlive", clrObject.ReadField<bool>("_keepAlive") },
        { "IsUpgraded", clrObject.ReadField<bool>("<IsUpgraded>k__BackingField") },
        { "HttpVersion", clrObject.ReadField<sbyte>("_httpVersion") },
        { "Remote.IPAddress", clrObject.ReadObjectField("<RemoteIpAddress>k__BackingField").IPAddress() },
        { "Remote.Port", clrObject.ReadField<int>("<RemotePort>k__BackingField") },
        { "Local.IPAddress", clrObject.ReadObjectField("<LocalIpAddress>k__BackingField").IPAddress() },
        { "Local.Port", clrObject.ReadField<int>("<LocalPort>k__BackingField") }
    };
    #endregion

}