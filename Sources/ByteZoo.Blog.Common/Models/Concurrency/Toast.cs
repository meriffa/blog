namespace ByteZoo.Blog.Common.Models.Concurrency;

/// <summary>
/// Toast
/// </summary>
public class Toast : Food
{

    #region Properties
    /// <summary>
    /// Toast spreads
    /// </summary>
    public required List<Spread> Spreads { get; set; }
    #endregion

}