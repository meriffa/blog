namespace ByteZoo.Blog.Common.Models.People;

/// <summary>
/// Picture
/// </summary>
public class Picture
{

    #region Properties
    /// <summary>
    /// Picture width
    /// </summary>
    public required int Width { get; set; }

    /// <summary>
    /// Picture height
    /// </summary>
    public required int Height { get; set; }

    /// <summary>
    /// Picture data
    /// </summary>
    public required int[] Data { get; set; }
    #endregion

}