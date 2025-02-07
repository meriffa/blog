using ByteZoo.Blog.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ByteZoo.Blog.Web.Controllers.Scenarios;

/// <summary>
/// Exceptions controller
/// </summary>
/// <param name="logger"></param>
[ApiController, Route("/Api/[controller]/[action]")]
public class ExceptionsController(ILogger<ExceptionsController> logger) : Controller
{

    #region Public Methods
    /// <summary>
    /// Exception (Unhandled Exception)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnhandledException"></exception>
    [HttpGet]
    public ActionResult Crash()
    {
        logger.LogInformation("Application crash started.");
        throw new UnhandledException("Application crash.");
    }
    #endregion

}