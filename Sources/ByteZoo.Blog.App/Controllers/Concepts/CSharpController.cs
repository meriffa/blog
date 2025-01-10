using ByteZoo.Blog.Common.Models.Business;
using ByteZoo.Blog.Common.Models.People;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// CSharp controller
/// </summary>
[Verb("Concepts-CSharp", HelpText = "C# operation.")]
public class CSharpController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var workforce = new Workforce<Employee>()
        {
            Employees = []
        };
        var employee = new Employee()
        {
            Id = 1,
            Name = new() { First = "John", Last = "Smith" },
            DateOfBirth = new DateTime(1989, 1, 12),
            EyeColor = PersonEyeColor.Blue,
            Events = [new EmployeeEvent(EmployeeEventType.Hired, DateOnly.FromDateTime(DateTime.Now))]
        };
        var employeeSummary = new { Name = employee.Name.Full, employee.Age };
        workforce.Employees.Add(employee);
        displayService.WriteInformation($"[Workforce] Employees = {workforce.Employees.Count}");
        displayService.WriteInformation($"[Workforce] Employee #1 = {workforce[employee.Name.Full]?.Name.Full}");
        displayService.WriteInformation($"[Employee] Summary = {employeeSummary}");
        displayService.Wait();
    }
    #endregion

}