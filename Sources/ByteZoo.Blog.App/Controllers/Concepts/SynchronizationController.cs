using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Synchronization controller
/// </summary>
[Verb("Concepts-Synchronization", HelpText = "Synchronization operation.")]
public class SynchronizationController : Controller
{

    #region Private Members
    private readonly object hashCodeInstance = new();
    private readonly object thinLockInstance = new();
    private readonly object syncBlkInstance = new();
    private readonly Lock lockInstance = new();
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        GenerateHashCode();
        Task.WaitAll(UseThinLock(), UseSyncBlk(), UseLock());
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Generate HashCode instance
    /// </summary>
    private void GenerateHashCode()
    {
        displayService.WriteInformation($"Hash Code = 0x{hashCodeInstance.GetHashCode():X6}");
    }

    /// <summary>
    /// Use Thin Lock instance
    /// </summary>
    /// <returns></returns>
    private Task UseThinLock()
    {
        return Task.Factory.StartNew(() =>
        {
            lock (thinLockInstance)
            {
                displayService.WriteInformation("Critical section (Thin Lock) begin.");
                displayService.Wait();
                displayService.WriteInformation("Critical section (Thin Lock) end.");
            }
        });
    }

    /// <summary>
    /// Use SyncBlk instance
    /// </summary>
    /// <returns></returns>
    private Task UseSyncBlk()
    {
        return Task.Factory.StartNew(() =>
        {
            syncBlkInstance.GetHashCode();
            lock (syncBlkInstance)
            {
                displayService.WriteInformation("Critical section (SyncBlk) begin.");
                displayService.Wait();
                displayService.WriteInformation("Critical section (SyncBlk) end.");
            }
        });
    }

    /// <summary>
    /// Use System.Threading.Lock instance
    /// </summary>
    /// <returns></returns>
    private Task UseLock()
    {
        return Task.Factory.StartNew(() =>
        {
            lock (lockInstance)
            {
                displayService.WriteInformation("Critical section (System.Threading.Lock) begin.");
                displayService.Wait();
                displayService.WriteInformation("Critical section (System.Threading.Lock) end.");
            }
        });
    }
    #endregion

}