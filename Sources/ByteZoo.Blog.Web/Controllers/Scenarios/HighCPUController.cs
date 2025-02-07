using Microsoft.AspNetCore.Mvc;

namespace ByteZoo.Blog.Web.Controllers.Scenarios;

/// <summary>
/// High CPU Usage controller
/// </summary>
/// <param name="logger"></param>
[ApiController, Route("/Api/[controller]/[action]")]
public class HighCPUController(ILogger<HighCPUController> logger) : Controller
{

    #region Public Methods
    /// <summary>
    /// High CPU Usage (Calculation)
    /// </summary>
    /// <param name="taskCount"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult Calculation(int taskCount)
    {
        var tasks = new Task<int>[taskCount];
        for (int i = 0; i < taskCount; i++)
            tasks[i] = StartCalculationTask(1024);
        Task.WaitAll(tasks);
        return Ok();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start calculation task
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    private Task<int> StartCalculationTask(int limit) => Task.Factory.StartNew(() =>
    {
        logger.LogInformation("Calculation task started (Thread ID = {ThreadId}).", Environment.CurrentManagedThreadId);
        var i = 0;
        while (i < limit)
            i = (i + 1) % limit;
        logger.LogInformation("Calculation task completed (Thread ID = {ThreadId}).", Environment.CurrentManagedThreadId);
        return i;
    });
    #endregion

}