namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Employee event
/// </summary>
public record EmployeeEvent(EmployeeEventType Type, DateOnly Date);