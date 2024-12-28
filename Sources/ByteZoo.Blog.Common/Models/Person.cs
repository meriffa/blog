namespace ByteZoo.Blog.Common.Models;

/// <summary>
/// Person
/// </summary>
public class Person : Entity
{

    #region Properties
    /// <summary>
    /// Person name
    /// </summary>
    public required PersonName Name { get; set; }

    /// <summary>
    /// Person date of birth
    /// </summary>
    public required DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Person age (years)
    /// </summary>
    public int Age => (DateTime.MinValue + DateTime.Now.Subtract(DateOfBirth)).Year - 1;

    /// <summary>
    /// Person eye color
    /// </summary>
    public required PersonEyeColor EyeColor { get; set; }
    #endregion

}