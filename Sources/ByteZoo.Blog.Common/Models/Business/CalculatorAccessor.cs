using System.Runtime.CompilerServices;

namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Calculator accessor
/// </summary>
public static class CalculatorAccessor
{

    #region Public Methods
    /// <summary>
    /// Return value field
    /// </summary>
    /// <param name="calculator"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "value")]
    public static extern ref int GetValueField(Calculator calculator);

    /// <summary>
    /// Return calculator value squared property
    /// </summary>
    /// <param name="calculator"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_ValueSquared")]
    public static extern int GetValueSquaredProperty(Calculator calculator);

    /// <summary>
    /// Return calculator value squared backing property
    /// </summary>
    /// <param name="calculator"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<ValueSquared>k__BackingField")]
    public static extern ref int GetValueSquaredBackingField(Calculator calculator);

    /// <summary>
    /// Return value multiple
    /// </summary>
    /// <param name="calculator"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetValueMultiple")]
    public static extern int GetValueMultiple(Calculator calculator, int value);
    #endregion

}