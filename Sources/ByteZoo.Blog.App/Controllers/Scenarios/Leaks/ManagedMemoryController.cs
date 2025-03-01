using ByteZoo.Blog.Common.Models.Business;
using ByteZoo.Blog.Common.Models.People;
using CommandLine;
using System.Security.Cryptography;

namespace ByteZoo.Blog.App.Controllers.Scenarios.Leaks;

/// <summary>
/// Managed Memory Leak controller
/// </summary>
[Verb("Scenarios-Leaks-ManagedMemory", HelpText = "Managed Memory Leak operation.")]
public partial class ManagedMemoryController : Controller
{

    #region Constants
    private const string TEXT_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    #endregion

    #region Private Members
    private static readonly Workforce<Employee> workforce = new() { Employees = [] };
    #endregion

    #region Properties
    /// <summary>
    /// Allocation batch size
    /// </summary>
    [Option('b', "allocationBatch", Default = 1, HelpText = "Allocation batch size.")]
    public int AllocationBatch { get; set; }

    /// <summary>
    /// Generated text length
    /// </summary>
    [Option('t', "textLength", Default = 10, HelpText = "Generated text length.")]
    public int TextLength { get; set; }

    /// <summary>
    /// Allocation delay
    /// </summary>
    [Option('d', "allocationDelay", Default = 1000, HelpText = "Allocation delay (ms).")]
    public int AllocationDelay { get; set; }

    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var task = StartMemoryConsumptionTask(AllocationBatch, TextLength, AllocationDelay, cancellationTokenSource.Token);
        displayService.Wait();
        cancellationTokenSource.Cancel();
        Task.Run(async () => await task).Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start memory consumption task
    /// </summary>
    /// <param name="allocationBatch"></param>
    /// <param name="textLength"></param>
    /// <param name="allocationDelay"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task StartMemoryConsumptionTask(int allocationBatch, int textLength, int allocationDelay, CancellationToken cancellationToken)
    {
        while (true)
        {
            for (int i = 0; i < allocationBatch; i++)
                workforce.Employees.Add(GetEmployee(textLength));
            displayService.WriteInformation($"Employee instances allocated (Count = {allocationBatch}, Total = {workforce.Employees.Count}).");
            if (cancellationToken.IsCancellationRequested)
                break;
            await Task.Delay(allocationDelay, cancellationToken);
        }
    }

    /// <summary>
    /// Return new employee
    /// </summary>
    /// <param name="textLength"></param>
    /// <returns></returns>
    private static Employee GetEmployee(int textLength) => new(GetText(textLength))
    {
        Id = Random.Shared.Next(),
        Name = new() { First = GetText(textLength), Last = GetText(textLength) },
        DateOfBirth = DateTime.Now,
        EyeColor = PersonEyeColor.Green,
        BaseSalary = Random.Shared.Next(),
        Events = []
    };

    /// <summary>
    /// Return random string
    /// </summary>
    /// <param name="textLength"></param>
    /// <returns></returns>
    private static string GetText(int textLength) => RandomNumberGenerator.GetString(TEXT_CHARACTERS, textLength);
    #endregion

}