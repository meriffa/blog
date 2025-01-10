namespace ByteZoo.Blog.Common.Models.Business;

/// <summary>
/// Workforce
/// </summary>
public class Workforce<T> where T : Employee
{

    #region Properties
    /// <summary>
    /// Workforce employees
    /// </summary>
    public required List<T> Employees { get; set; }
    #endregion

    #region Indexers
    /// <summary>
    /// Return employee by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index] => Employees[index];

    /// <summary>
    /// Return employee by full name
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public T? this[string fullName] => Employees.FirstOrDefault(i => i.Name.Full == fullName);
    #endregion

}