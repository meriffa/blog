using ByteZoo.Blog.Common.Models.Business;
using ByteZoo.Blog.Common.Models.People;
using CommandLine;
using System.Reflection;

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
        var employee = GetEmployee();
        var workforce = GetWorkforce(employee);
        var employeeSummary = new { Name = employee.Name.Full, employee.Age };
        var (eventCount, minEventDate, maxEventDate) = GetEmployeeEventSummary(employee, EmployeeEventType.Promoted);
        var stillEmployed = IsStillEmployed(employee);
        var baseSalaryNormalized = GetNormalizedBaseSalary(employee);
        var baseSalaryTransformed = GetTransformedBaseSalary(employee);
        var baseSalaryAdjusted = AdjustEmployeeBaseSalary(employee, 0.035m);
        displayService.WriteInformation($"[Workforce] Employees = {workforce.Employees.Count}");
        displayService.WriteInformation($"[Workforce] Employee #1 = {workforce[employee.Name.Full]?.Name.Full}");
        displayService.WriteInformation($"[Employee] Summary = {employeeSummary}, Is Employed = {stillEmployed}, Salary = {baseSalaryNormalized:c}, {baseSalaryTransformed}");
        displayService.WriteInformation($"[Employee] Promotions = {eventCount}, First Promotion = {minEventDate:d}, Last Promotion = {maxEventDate:d}");
        displayService.WriteInformation($"[Employee] Adjusted Salary = {baseSalaryAdjusted?.Value:c}, Date = {baseSalaryAdjusted?.Date:d}");
        ChangeEmployeePosition(employee);
        DisplayEmployeeSchema(typeof(Employee));
        DisplayCalculator(111, 11);
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return employee instance
    /// </summary>
    /// <returns></returns>
    private static Employee GetEmployee() => new("Engineer")
    {
        Id = 1,
        Name = new() { First = "John", Last = "Smith" },
        DateOfBirth = new DateTime(1989, 1, 12),
        EyeColor = PersonEyeColor.Blue,
        BaseSalary = 123_456.78m,
        Events =
        [
            new EmployeeEvent(EmployeeEventType.Hired, DateOnly.FromDateTime(new DateTime(2005, 6, 1))),
            new EmployeeEvent(EmployeeEventType.Promoted, DateOnly.FromDateTime(new DateTime(2010, 7, 1))),
            new EmployeeEvent(EmployeeEventType.Promoted, DateOnly.FromDateTime(new DateTime(2015, 8, 1))),
            new EmployeeEvent(EmployeeEventType.Promoted, DateOnly.FromDateTime(new DateTime(2020, 9, 1)))
        ]
    };

    /// <summary>
    /// Return workforce instance
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    private static Workforce<Employee> GetWorkforce(Employee employee) => new() { Employees = [employee] };

    /// <summary>
    /// Return employee event summary (deconstructing tuples)
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    private static (int count, DateOnly? minDate, DateOnly? maxDate) GetEmployeeEventSummary(Employee employee, EmployeeEventType eventType)
    {
        var events = employee.Events.Where(i => i.Type == eventType);
        var count = events.Count();
        if (count > 0)
        {
            var minDate = events.Select(i => i.Date).Min();
            var maxDate = events.Select(i => i.Date).Max();
            return (count, minDate, maxDate);
        }
        return (count, null, null);
    }

    /// <summary>
    /// Check if employee is still employed (pattern matching)
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    private static string? IsStillEmployed(Employee employee)
    {
        var eventType = employee.Events.LastOrDefault();
        if (eventType is not null)
            return eventType.Type switch
            {
                EmployeeEventType.Hired or EmployeeEventType.Promoted or EmployeeEventType.Demoted => "Yes",
                EmployeeEventType.OnLeave or EmployeeEventType.Resigned or EmployeeEventType.Terminated => "No",
                _ => throw new Exception($"Employee event type {eventType} is not supported.")
            };
        return null;
    }

    /// <summary>
    /// Return normalized base salary (casting)
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    private static double GetNormalizedBaseSalary(Employee employee)
    {
        var baseSalary = (double)employee.BaseSalary / 100.0d;
        return Math.Floor(baseSalary) * 100.0d;
    }

    /// <summary>
    /// Return transformed base salary (boxing & unboxing)
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    private static EmployeePayment GetTransformedBaseSalary(Employee employee)
    {
        object result = new EmployeePayment { Value = employee.BaseSalary, Date = employee.Events.First().Date };
        return (EmployeePayment)result;
    }

    /// <summary>
    /// Change employee position
    /// </summary>
    /// <param name="employee"></param>
    private void ChangeEmployeePosition(Employee employee)
    {
        employee.PositionChanged += EmployeePositionChanged;
        employee.ChangePosition("Senior Engineer");
    }

    /// <summary>
    /// Handle employee position changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EmployeePositionChanged(object? sender, PositionChangedArgs e)
    {
        if (sender is Employee employee)
            displayService.WriteInformation($"[Employee] Name = {employee.Name.Full}, From = {e.OriginalPosition}, To = {e.CurrentPosition}");
    }

    /// <summary>
    /// Display employee schema
    /// </summary>
    /// <param name="type"></param>
    private void DisplayEmployeeSchema(Type type)
    {
        if (type.GetCustomAttribute<EmployeeSchemaAttribute>() is EmployeeSchemaAttribute attribute)
            displayService.WriteInformation($"[Employee] Schema Version = {attribute.Version}");
    }

    /// <summary>
    /// Adjust employee base salary
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="inflation"></param>
    /// <returns></returns>
    private static EmployeePayment? AdjustEmployeeBaseSalary(Employee employee, decimal inflation)
    {
        var value = employee.BaseSalary * (1.00m + inflation);
        var result = new EmployeePayment { Value = value, Date = DateOnly.FromDateTime(DateTime.Now) };
        return result;
    }

    /// <summary>
    /// Display calculator
    /// </summary>
    /// <param name="initialValue"></param>
    /// <param name="modifiedValue"></param>
    private void DisplayCalculator(int initialValue, int modifiedValue)
    {
        var calculator = new Calculator(initialValue);
        CalculatorAccessor.GetValueField(calculator) = modifiedValue;
        CalculatorAccessor.GetValueSquaredBackingField(calculator) = modifiedValue * modifiedValue;
        var modified = CalculatorAccessor.GetValueField(calculator);
        var doubled = CalculatorAccessor.GetValueMultiple(calculator, 2);
        var squared = CalculatorAccessor.GetValueSquaredProperty(calculator);
        displayService.WriteInformation($"[Calculator] Original = {initialValue}, Modified = {modified}, Doubled = {doubled}, Squared = {squared}");
    }
    #endregion

}