using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Scenarios.Hangs;

/// <summary>
/// Application Hang (Deadlock, System.Threading.Lock) controller
/// </summary>
[Verb("Scenarios-Hangs-Lock", HelpText = "Application Hang (Deadlock, System.Threading.Lock) operation.")]
public partial class LockController : Controller
{

    #region Private Members
    private static readonly Lock sharedResource1 = new();
    private static readonly Lock sharedResource2 = new();
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var task1 = Task.Factory.StartNew(() => StartSharedResourceTask(sharedResource1, 1, sharedResource2, 2));
        var task2 = Task.Factory.StartNew(() => StartSharedResourceTask(sharedResource2, 2, sharedResource1, 1));
        Task.WaitAll(task1, task2);
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
    /// <param name="delay"></param>
    private void StartSharedResourceTask(Lock resource1, int resourceIndex1, Lock resource2, int resourceIndex2, int delay = 1000)
    {
        displayService.WriteInformation($"Shared resource #{resourceIndex1} lock pending (Thread ID = {Environment.CurrentManagedThreadId}).");
        lock (resource1)
        {
            displayService.WriteInformation($"Shared resource #{resourceIndex1} lock acquired (Thread ID = {Environment.CurrentManagedThreadId}).");
            Thread.Sleep(delay);
            displayService.WriteInformation($"Shared resource #{resourceIndex2} lock pending (Thread ID = {Environment.CurrentManagedThreadId}).");
            lock (resource2)
            {
                displayService.WriteInformation($"Shared resource #{resourceIndex2} lock acquired (Thread ID = {Environment.CurrentManagedThreadId}).");
                Thread.Sleep(delay);
            }
            displayService.WriteInformation($"Shared resource #{resourceIndex2} lock released (Thread ID = {Environment.CurrentManagedThreadId}).");
        }
        displayService.WriteInformation($"Shared resource #{resourceIndex1} lock released (Thread ID = {Environment.CurrentManagedThreadId}).");
    }
    #endregion

}