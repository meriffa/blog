using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject System.Security.Cryptography extension methods
/// </summary>
public static class ClrObjectCryptographyExtensions
{

    #region Public Methods
    /// <summary>
    /// Return certificate serial number
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static string? SerialNumber(this ClrObject clrObject)
    {
        var serialNumber = clrObject.ReadObjectField("_lazySerialNumber");
        if (!serialNumber.IsNull && serialNumber.IsArray)
        {
            var array = serialNumber.AsArray();
            var result = new byte[array.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = array.GetValue<byte>(i);
            return BitConverter.ToString(result).Replace('-', ' ');
        }
        return null;
    }
    #endregion

}