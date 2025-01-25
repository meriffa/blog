namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Employee event
/// </summary>
/// <param name="Type"></param>
/// <param name="Date"></param>
public record EmployeeEvent(EmployeeEventType Type, DateOnly Date);