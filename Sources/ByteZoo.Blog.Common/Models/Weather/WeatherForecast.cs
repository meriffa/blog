namespace ByteZoo.Blog.Common.Models.Weather;

/// <summary>
/// Weather forecast
/// </summary>
/// <param name="Date"></param>
/// <param name="TemperatureCelsius"></param>
/// <param name="Summary"></param>
public record WeatherForecast(DateOnly Date, int TemperatureCelsius, string Summary)
{

    #region Properties
    /// <summary>
    /// Temperature [Fahrenheit]
    /// </summary>
    public int TemperatureFahrenheit => 32 + (int)(TemperatureCelsius / 0.5556);
    #endregion

}