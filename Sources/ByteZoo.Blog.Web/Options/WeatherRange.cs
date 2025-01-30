namespace ByteZoo.Blog.Web.Options;

/// <summary>
/// Weather range
/// </summary>
public class WeatherRange
{

    #region Properties
    /// <summary>
    /// Weather range minimum
    /// </summary>
    public required int Minimum { get; set; }

    /// <summary>
    /// Weather range maximum
    /// </summary>
    public required int Maximum { get; set; }
    #endregion

}