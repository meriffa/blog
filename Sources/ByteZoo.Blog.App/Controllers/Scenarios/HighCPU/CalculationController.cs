using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Scenarios.HighCPU;

/// <summary>
/// High CPU Usage (Calculation) controller
/// </summary>
[Verb("Scenarios-HighCPU-Calculation", HelpText = "High CPU Usage (Calculation) operation.")]
public partial class CalculationController : Controller
{

    #region Properties
    /// <summary>
    /// Number of calculation tasks
    /// </summary>
    [Option('t', "taskCount", Default = 1, HelpText = "Number of calculation tasks.")]
    public int TaskCount { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var tasks = new Task<int>[TaskCount];
        for (int i = 0; i < TaskCount; i++)
            tasks[i] = StartCalculationTask(1024);
        Task.WaitAll(tasks);
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
        displayService.WriteInformation($"Calculation task started (Thread ID = {Environment.CurrentManagedThreadId}).");
        var i = 0;
        while (i < limit)
            i = (i + 1) % limit;
        return i;
    });
    #endregion

}