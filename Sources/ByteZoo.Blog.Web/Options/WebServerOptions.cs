namespace ByteZoo.Blog.Web.Options;

/// <summary>
/// Web server options
/// </summary>
public class WebServerOptions
{

    #region Properties
    /// <summary>
    /// Web server options weather range
    /// </summary>
    public required WeatherRange WeatherRange { get; set; }
    #endregion

}