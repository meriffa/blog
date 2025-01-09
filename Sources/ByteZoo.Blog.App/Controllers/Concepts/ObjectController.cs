using ByteZoo.Blog.Common.Models.People;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Object controller
/// </summary>
[Verb("Concepts-Object", HelpText = "Object operation.")]
public class ObjectController : Controller
{

    #region Properties
    /// <summary>
    /// Person first name
    /// </summary>
    [Option('f', "firstName", Required = true, HelpText = "Person first name.")]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Person last name
    /// </summary>
    [Option('l', "lastName", Required = true, HelpText = "Person last name.")]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Person date of birth
    /// </summary>
    [Option('d', "dateOfBirth", Required = true, HelpText = "Person date of birth.")]
    public DateTime DateOfBirth { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var person = GetPerson();
        DisplayPerson(person);
        displayService.Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return person instance
    /// </summary>
    /// <returns></returns>
    private Person GetPerson()
    {
        var id = Random.Shared.Next(1, 1000);
        return new Person()
        {
            Id = id,
            Name = new()
            {
                First = FirstName,
                Last = LastName
            },
            DateOfBirth = DateOfBirth,
            EyeColor = GetPersonEyeColor(id)
        };
    }

    /// <summary>
    /// Display person instance (using string concatenation and interpolation)
    /// </summary>
    /// <param name="person"></param>
    private void DisplayPerson(Person person) => displayService.WriteInformation("Person: " +
        $"ID = {person.Id}, " +
        $"First Name = '{person.Name.First}', " +
        $"Last Name = '{person.Name.Last}', " +
        $"Full Name = '{person.Name.Full}', " +
        $"Date Of Birth = {person.DateOfBirth:d}, " +
        $"Age = {person.Age}, " +
        $"Eye Color = {person.EyeColor}");

    /// <summary>
    /// Return person eye color
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private static PersonEyeColor GetPersonEyeColor(int id) => id switch
    {
        <= 600 => PersonEyeColor.Brown,
        > 600 and <= 900 => PersonEyeColor.Green,
        _ => PersonEyeColor.Blue
    };
    #endregion

}