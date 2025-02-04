using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// AsyncLocal controller
/// </summary>
[Verb("Concepts-AsyncLocal", HelpText = "AsyncLocal operation.")]
public class AsyncLocalController : Controller
{

    #region Private Members
    private static readonly ThreadLocal<string[]> threadLocal = new();
    private static readonly AsyncLocal<string[]> asyncLocal = new();
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        Task.Run(async () =>
        {
            var count = 10;
            asyncLocal.Value = GetValues("Async Local Value A", count);
            threadLocal.Value = GetValues("Thread Local Value A", count);
            var task1 = ExecuteTaskAsync("Value 1");
            asyncLocal.Value = GetValues("Async Local Value B", count);
            threadLocal.Value = GetValues("Thread Local Value B", count);
            var task2 = ExecuteTaskAsync("Value 2");
            await task1;
            await task2;
        }).Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return local values
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private static string[] GetValues(string prefix, int count) => [.. Enumerable.Range(1, count).Select(i => $"{prefix} - {i}")];

    /// <summary>
    /// Execute asynchronous task
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private async Task ExecuteTaskAsync(string value)
    {
        displayService.WriteInformation($"Task started (Thread ID = {Environment.CurrentManagedThreadId}, Parameter = '{value}', AsyncLocal = '{asyncLocal.Value?[0]}', ThreadLocal = '{threadLocal.Value?[0]}').");
        await Task.Delay(Random.Shared.Next(500, 1500));
        displayService.Wait();
        displayService.WriteInformation($"Task completed (Thread ID = {Environment.CurrentManagedThreadId}, Parameter = '{value}', AsyncLocal = '{asyncLocal.Value?[0]}', ThreadLocal = '{threadLocal.Value?[0]}').");
    }
    #endregion

}