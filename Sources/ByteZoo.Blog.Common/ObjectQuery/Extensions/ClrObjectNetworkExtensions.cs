using Microsoft.Diagnostics.Runtime;

namespace ByteZoo.Blog.Common.ObjectQuery.Extensions;

/// <summary>
/// ClrObject network extension methods
/// </summary>
public static class ClrObjectNetworkExtensions
{

    #region Public Methods
    /// <summary>
    /// Return IPAddress value
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static string IPAddress(this ClrObject clrObject)
    {
        var bytes = BitConverter.GetBytes(clrObject.ReadField<uint>("_addressOrScopeId"));
        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
    }

    /// <summary>
    /// Return IPAddress numbers
    /// </summary>
    /// <param name="clrObject"></param>
    /// <returns></returns>
    public static short[]? IPAddressNumbers(this ClrObject clrObject)
    {
        var numbers = clrObject.ReadObjectField("_numbers");
        if (!numbers.IsNull && numbers.IsArray)
        {
            var array = numbers.AsArray();
            var result = new short[array.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = array.GetValue<short>(i);
            return result;
        }
        return null;
    }
    #endregion

}