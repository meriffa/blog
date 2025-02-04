using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// ThreadLocal controller
/// </summary>
[Verb("Concepts-ThreadLocal", HelpText = "ThreadLocal operation.")]
public class ThreadLocalController : Controller
{

    #region Private Members
    private static readonly ThreadLocal<string> threadLocal = new();
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var task1 = Task.Factory.StartNew(() => ExecuteTask("Task A"));
        var task2 = Task.Factory.StartNew(() => ExecuteTask("Task B"));
        Task.WaitAll(task1, task2);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Execute task
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private void ExecuteTask(string value)
    {
        threadLocal.Value = $"{value} - {Environment.CurrentManagedThreadId}";
        displayService.WriteInformation($"Task started (Thread ID = {Environment.CurrentManagedThreadId}, ThreadLocal.Value = '{threadLocal.Value}').");
        displayService.Wait();
    }
    #endregion

}