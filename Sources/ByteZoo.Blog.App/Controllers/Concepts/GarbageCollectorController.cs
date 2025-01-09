using ByteZoo.Blog.Common.Models.Meals;
using ByteZoo.Blog.Common.Models.People;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Garbage Collector controller
/// </summary>
[Verb("Concepts-GarbageCollector", HelpText = "Garbage Collector operation.")]
public class GarbageCollectorController : Controller
{

    #region Private Members
    private Person? person;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        person = new()
        {
            Id = 1,
            Name = new() { First = "John", Last = "Smith" },
            DateOfBirth = new DateTime(1980, 1, 1),
            EyeColor = PersonEyeColor.Green
        };
        DisplayPerson(person);
        GC.Collect(2, GCCollectionMode.Forced, true, true);
        DisplayBreakfast();
        var picture = GetPicture(100, 250);
        displayService.Wait();
        DisplayPicture(picture);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display person
    /// </summary>
    /// <param name="person"></param>
    private void DisplayPerson(Person person) => displayService.WriteInformation($"[Person] Full Name = '{person.Name.Full}'.");

    /// <summary>
    /// Display breakfast
    /// </summary>
    private void DisplayBreakfast()
    {
        using var breakfast = new Breakfast() { Drinks = [], Food = [] };
        displayService.WriteInformation($"[Breakfast] Drinks = {breakfast.Drinks?.Count}, Food = {breakfast.Food?.Count}.");
    }

    /// <summary>
    /// Return picture
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private static Picture GetPicture(int width, int height) => new() { Width = width, Height = height, Data = new int[width * height] };

    /// <summary>
    /// Display picture
    /// </summary>
    /// <param name="picture"></param>
    private void DisplayPicture(Picture picture) => displayService.WriteInformation($"[Picture] Width = {picture.Width}, Height = {picture.Height}");
    #endregion

}