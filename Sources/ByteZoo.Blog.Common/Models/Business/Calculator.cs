namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Calculator
/// </summary>
/// <param name="value"></param>
public class Calculator(int value)
{

    #region Private Members
    private readonly int value = value;
    #endregion

    #region Properties
    /// <summary>
    /// Calculator value squared
    /// </summary>
    private int ValueSquared { get; } = value * value;
    #endregion

    #region Private Methods
    /// <summary>
    /// Return value multiple
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int GetValueMultiple(int value) => this.value * value;
    #endregion

}