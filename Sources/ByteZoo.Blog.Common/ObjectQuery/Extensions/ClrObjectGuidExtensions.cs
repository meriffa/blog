using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject Guid extension methods
/// </summary>
public static class ClrObjectGuidExtensions
{

    #region Public Methods
    /// <summary>
    /// Return Guid value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static string Guid(this ClrObject clrObject) => clrObject.ReadBoxedValue<Guid>().ToString();
    #endregion

}