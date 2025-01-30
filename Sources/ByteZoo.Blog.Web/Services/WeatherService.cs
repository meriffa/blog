using ByteZoo.Blog.Common.Models.Weather;
using ByteZoo.Blog.Web.Interfaces;
using ByteZoo.Blog.Web.Options;
using Microsoft.Extensions.Options;

namespace ByteZoo.Blog.Web.Services;

/// <summary>
/// Weather service
/// </summary>
/// <param name="options"></param>
public class WeatherService(IOptions<WebServerOptions> options) : IWeatherService
{

    #region Private Members
    private static readonly string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
    #endregion

    #region Public Methods
    /// <summary>
    /// Generate weather forecast
    /// </summary>
    /// <param name="numberOfDays"></param>
    /// <returns></returns>
    public WeatherForecast[] GenerateForecast(int numberOfDays) => [.. Enumerable.Range(1, numberOfDays).Select(dayIndex => new WeatherForecast
    (
        DateOnly.FromDateTime(DateTime.Now.AddDays(dayIndex)),
        Random.Shared.Next(options.Value.WeatherRange.Minimum, options.Value.WeatherRange.Maximum),
        summaries[Random.Shared.Next(summaries.Length)]
    ))];
    #endregion

}