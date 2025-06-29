using Microsoft.AspNetCore.Mvc;

namespace ByteZoo.Blog.Web.Controllers.Scenarios;

/// <summary>
/// Application Hangs controller
/// </summary>
/// <param name="logger"></param>
[ApiController, Route("/Api/[controller]/[action]")]
public class HangsController(ILogger<HangsController> logger) : Controller
{

    #region Public Methods
    /// <summary>
    /// Application Hang (Deadlock, System.Threading.Lock)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<bool> Lock()
    {
        var sharedResource1 = new Lock();
        var sharedResource2 = new Lock();
        // Use TaskCreationOptions.LongRunning to prevent Thread reuse.
        var task1 = Task.Factory.StartNew(() => StartSharedResourceTask(sharedResource1, 1, sharedResource2, 2, HttpContext.TraceIdentifier), TaskCreationOptions.LongRunning);
        var task2 = Task.Factory.StartNew(() => StartSharedResourceTask(sharedResource2, 2, sharedResource1, 1, HttpContext.TraceIdentifier), TaskCreationOptions.LongRunning);
        Task.WaitAll(task1, task2);
        return Ok(true);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start shared resource task
    /// </summary>
    /// <param name="resource1"></param>
    /// <param name="resourceIndex1"></param>
    /// <param name="resource2"></param>
    /// <param name="resourceIndex2"></param>
    /// <param name="traceIdentifier"></param>
    /// <param name="delay"></param>
    private void StartSharedResourceTask(Lock resource1, int resourceIndex1, Lock resource2, int resourceIndex2, string traceIdentifier, int delay = 1000)
    {
        logger.LogInformation("Shared resource #{resourceIndex} lock pending (Request ID = {traceIdentifier}, Thread ID = {ThreadId}).", resourceIndex1, traceIdentifier, Environment.CurrentManagedThreadId);
        lock (resource1)
        {
            logger.LogInformation("Shared resource #{resourceIndex} lock acquired (Request ID = {traceIdentifier}, Thread ID = {ThreadId}).", resourceIndex1, traceIdentifier, Environment.CurrentManagedThreadId);
            Thread.Sleep(delay);
            logger.LogInformation("Shared resource #{resourceIndex} lock pending (Request ID = {traceIdentifier}, Thread ID = {ThreadId}).", resourceIndex2, traceIdentifier, Environment.CurrentManagedThreadId);
            lock (resource2)
            {
                logger.LogInformation("Shared resource #{resourceIndex} lock acquired (Request ID = {traceIdentifier}, Thread ID = {ThreadId}).", resourceIndex2, traceIdentifier, Environment.CurrentManagedThreadId);
                Thread.Sleep(delay);
            }
            logger.LogInformation("Shared resource #{resourceIndex} lock released (Request ID = {traceIdentifier}, Thread ID = {ThreadId}).", resourceIndex2, traceIdentifier, Environment.CurrentManagedThreadId);
        }
        logger.LogInformation("Shared resource #{resourceIndex} lock released (Request ID = {traceIdentifier}, Thread ID = {ThreadId}).", resourceIndex1, traceIdentifier, Environment.CurrentManagedThreadId);
    }
    #endregion

}