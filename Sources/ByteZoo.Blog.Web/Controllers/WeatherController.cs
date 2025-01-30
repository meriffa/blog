using ByteZoo.Blog.Common.Models.Weather;
using ByteZoo.Blog.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ByteZoo.Blog.Web.Controllers;

/// <summary>
/// Weather controller
/// </summary>
[ApiController, Route("/Api/[controller]/[action]")]
public class WeatherController() : Controller
{

    #region Public Methods
    /// <summary>
    /// Generate weather forecast
    /// </summary>
    /// <param name="service"></param>
    /// <param name="numberOfDays"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<WeatherForecast[]> GenerateForecast([FromServices] IWeatherService service, int numberOfDays)
    {
        try
        {
            return Ok(service.GenerateForecast(numberOfDays));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}