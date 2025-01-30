using ByteZoo.Blog.Common.Models.Weather;

namespace ByteZoo.Blog.Web.Interfaces;

/// <summary>
/// Weather service interface
/// </summary>
public interface IWeatherService
{

    #region Public Methods
    /// <summary>
    /// Generate weather forecast
    /// </summary>
    /// <param name="numberOfDays"></param>
    /// <returns></returns>
    WeatherForecast[] GenerateForecast(int numberOfDays);
    #endregion

}