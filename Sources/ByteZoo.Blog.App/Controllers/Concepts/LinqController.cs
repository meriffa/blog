using ByteZoo.Blog.Common.Models.Business;
using ByteZoo.Blog.Common.Models.People;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// LINQ controller
/// </summary>
[Verb("Concepts-Linq", HelpText = "LINQ operation.")]
public class LinqController : Controller
{

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var employees = GetEmployees();
        var selectedEmployees = employees.Where(i => i.DateOfBirth.Year >= 1985 && i.Events.Exists(i => i.Type == EmployeeEventType.Hired));
        var employeeSelection = selectedEmployees.Select(i => new { FullName = i.Name.Full, Salary = i.BaseSalary });
        var averageSalary = employeeSelection.Average(i => i.Salary);
        var salaryRanking = employeeSelection.OrderBy(i => i.Salary);
        var selectedEmployee = salaryRanking.Skip(employeeSelection.Count() / 2).First();
        foreach (var employee in salaryRanking)
            displayService.WriteInformation($"Employee: Name = '{employee.FullName}', Salary = {employee.Salary:c}");
        displayService.WriteInformation($"Salary: Name = '{selectedEmployee.FullName}', Difference = {selectedEmployee.Salary / averageSalary:p0}");
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return employee list
    /// </summary>
    /// <returns></returns>
    private static List<Employee> GetEmployees() =>
    [
        new("Software Developer")
        {
            Id = 1,
            Name = new() { First = "John", Last = "Smith" },
            DateOfBirth = new DateTime(1985, 6, 1),
            EyeColor = PersonEyeColor.Green,
            BaseSalary = 103_000,
            Events = [ new(EmployeeEventType.Hired, new DateOnly(2010, 7, 2)) ]
        },
        new("Project Manager")
        {
            Id = 2,
            Name = new() { First = "Jane", Last = "Smith" },
            DateOfBirth = new DateTime(1987, 8, 1),
            EyeColor = PersonEyeColor.Green,
            BaseSalary = 109_000,
            Events = [ new(EmployeeEventType.Hired, new DateOnly(2012, 8, 3)), new(EmployeeEventType.Promoted, new DateOnly(2013, 8, 3)) ]
        },
        new("Senior QA Analyst")
        {
            Id = 3,
            Name = new() { First = "George", Last = "Brown" },
            DateOfBirth = new DateTime(1989, 10, 1),
            EyeColor = PersonEyeColor.Green,
            BaseSalary = 93_000,
            Events = [ new(EmployeeEventType.Hired, new DateOnly(2014, 9, 4)) ]
        }
    ];
    #endregion

}