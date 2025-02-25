using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject extension methods
/// </summary>
public static class ClrObjectExtensions
{

    #region Public Methods
    /// <summary>
    /// Return type name
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static string TypeName(this ClrObject clrObject) => clrObject.Type?.Name ?? string.Empty;

    /// <summary>
    /// Return address (hex)
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static string AddressHex(this ClrObject clrObject) => $"0x{clrObject.Address:X16}";

    /// <summary>
    /// Check if type object base type
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static bool Inherits(this ClrObject clrObject, string baseType) => Inherits(clrObject.Type, baseType);

    /// <summary>
    /// Check if object implements interface type
    /// </summary>
    /// <param name="clrObject"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    public static bool Implements(this ClrObject clrObject, string interfaceType) => Implements(clrObject.Type, interfaceType);
    #endregion

    #region Private Members
    /// <summary>
    /// Check if type inherits base type
    /// </summary>
    /// <param name="clrType"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    private static bool Inherits(ClrType? clrType, string baseType)
    {
        if (clrType == null)
            return false;
        else if (clrType.Name == baseType)
            return true;
        else
            return Inherits(clrType.BaseType, baseType);
    }

    /// <summary>
    /// Check if type implements interface type
    /// </summary>
    /// <param name="clrType"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    private static bool Implements(ClrType? clrType, string interfaceType)
    {
        if (clrType == null)
            return false;
        foreach (var clrInterface in clrType.EnumerateInterfaces())
            if (clrInterface.Name == interfaceType)
                return true;
        return Implements(clrType.BaseType, interfaceType);
    }
    #endregion

}