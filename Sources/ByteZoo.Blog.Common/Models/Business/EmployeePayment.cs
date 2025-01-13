namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Employee payment
/// </summary>
public struct EmployeePayment
{

    #region Public Members
    public decimal Value;
    public DateOnly Date;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return instance text representation
    /// </summary>
    /// <returns></returns>
    public override readonly string ToString() => "Amount = " + Value.ToString("c");
    #endregion

}