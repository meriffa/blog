using ByteZoo.Blog.Common.Models;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Thread controller
/// </summary>
[Verb("Concepts-Thread", HelpText = "Thread operation.")]
public class ThreadController : Controller
{

    #region Properties
    /// <summary>
    /// Thread type
    /// </summary>
    [Option('t', "type", Required = false, Default = ThreadType.Thread, HelpText = "Thread data.")]
    public ThreadType Type { get; set; }

    /// <summary>
    /// Thread data
    /// </summary>
    [Option('d', "data", Required = true, HelpText = "Thread data.")]
    public string Data { get; set; } = null!;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        if (Type == ThreadType.Thread)
            ExecuteThread();
        else if (Type == ThreadType.Task)
            ExecuteTask();
        displayService.WriteInformation($"Resuming main thread execution.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Execute thread
    /// </summary>
    private void ExecuteThread()
    {
        var thread = new Thread((data) =>
        {
            displayService.WriteInformation($"Thread started (Thread ID = {Environment.CurrentManagedThreadId}, Data = '{data}').");
            displayService.Wait();
            displayService.WriteInformation($"Thread completed (Thread ID = {Environment.CurrentManagedThreadId}).");
        });
        thread.Start(Data);
        thread.Join();
    }

    /// <summary>
    /// Execute task
    /// </summary>
    private void ExecuteTask() => Task.Factory.StartNew((data) =>
    {
        displayService.WriteInformation($"Task started (Thread ID = {Environment.CurrentManagedThreadId}, Data = '{data}').");
        displayService.Wait();
        displayService.WriteInformation($"Task completed (Thread ID = {Environment.CurrentManagedThreadId}).");
    }, Data).Wait();
    #endregion

}