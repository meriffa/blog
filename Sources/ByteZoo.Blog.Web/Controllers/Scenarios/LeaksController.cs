using ByteZoo.Blog.Common.Models.Business;
using ByteZoo.Blog.Common.Models.People;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ByteZoo.Blog.Web.Controllers.Scenarios;

/// <summary>
/// Resource Leaks controller
/// </summary>
/// <param name="logger"></param>
[ApiController, Route("/Api/[controller]/[action]")]
public class LeaksController(ILogger<LeaksController> logger) : Controller
{

    #region Constants
    private const string TEXT_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    #endregion

    #region Private Members
    private static readonly Workforce<Employee> workforce = new() { Employees = [] };
    #endregion

    #region Public Methods
    /// <summary>
    /// Managed Memory Leak
    /// </summary>
    /// <param name="allocationBatch"></param>
    /// <param name="textLength"></param>
    /// <param name="allocationDelay"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult ManagedMemory(int allocationBatch, int textLength, int allocationDelay, int duration)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var task = StartMemoryConsumptionTask(allocationBatch, textLength, allocationDelay, cancellationTokenSource.Token);
        Thread.Sleep(TimeSpan.FromSeconds(duration));
        cancellationTokenSource.Cancel();
        Task.Run(async () => await task).Wait();
        return Ok();
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
            logger.LogInformation("Employee instances allocated (Count = {allocationBatch}, Total = {workforce.Employees.Count}).", allocationBatch, workforce.Employees.Count);
            if (cancellationToken.IsCancellationRequested)
                break;
            await Task.Delay(allocationDelay);
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
    /// REturn random string
    /// </summary>
    /// <param name="textLength"></param>
    /// <returns></returns>
    private static string GetText(int textLength) => RandomNumberGenerator.GetString(TEXT_CHARACTERS, textLength);
    #endregion

}