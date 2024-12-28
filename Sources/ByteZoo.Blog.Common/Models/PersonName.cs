namespace ByteZoo.Blog.Common.Models;

/// <summary>
/// Person name
/// </summary>
public class PersonName
{

    #region Properties
    /// <summary>
    /// Person first name
    /// </summary>
    public required string First { get; set; }

    /// <summary>
    /// Person last name
    /// </summary>
    public required string Last { get; set; }

    /// <summary>
    /// Person full name
    /// </summary>
    public string Full => $"{First} {Last}";
    #endregion

}