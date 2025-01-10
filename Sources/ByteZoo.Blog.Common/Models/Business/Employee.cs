using ByteZoo.Blog.Common.Models.People;

namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Employee
/// </summary>
public class Employee : Person
{

    #region Properties
    /// <summary>
    /// Employee events
    /// </summary>
    public required List<EmployeeEvent> Events { get; set; }
    #endregion

}