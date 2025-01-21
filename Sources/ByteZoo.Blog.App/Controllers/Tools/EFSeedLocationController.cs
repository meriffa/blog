using ByteZoo.Blog.Common.EntityFramework.Contexts;
using ByteZoo.Blog.Common.EntityFramework.Models;
using CommandLine;
using Microsoft.EntityFrameworkCore;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Entity Framework location seed controller
/// </summary>
[Verb("Tools-EFSeedLocation", HelpText = "Entity Framework seed Location operation.")]
public class EFSeedLocationController : EFController
{

    #region Properties
    /// <summary>
    /// Minimum number of locations per company
    /// </summary>
    [Option("locationsPerCompanyMin", Required = true, HelpText = "Minimum number of locations per company.")]
    public int LocationsPerCompanyMin { get; set; }

    /// <summary>
    /// Maximum number of locations per company
    /// </summary>
    [Option("locationsPerCompanyMax", Required = true, HelpText = "Maximum number of locations per company.")]
    public int LocationsPerCompanyMax { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var context = new DatabaseContext(Type, ConnectionString);
        foreach (var companyId in GetCompanyIds())
        {
            var locationsPerCompany = Random.Shared.Next(LocationsPerCompanyMin, LocationsPerCompanyMax + 1);
            var company = context.Companies.Include(i => i.Locations).First(i => i.Id == companyId);
            for (int index = 1; index <= locationsPerCompany; index++)
                company.Locations.Add(new Location() { Id = 0, Company = company, Name = GetLocationName(company.Name, index), Type = GetLocationType() });
            context.SaveChanges();
            displayService.WriteInformation($"Company locations created (Company ID = {company.Id}, Locations = {locationsPerCompany}).");
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return company id list
    /// </summary>
    /// <returns></returns>
    private List<int> GetCompanyIds()
    {
        using var context = new DatabaseContext(Type, ConnectionString);
        return [.. context.Companies.Select(i => i.Id)];
    }

    /// <summary>
    /// Return location name
    /// </summary>
    /// <param name="companyName"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string GetLocationName(string companyName, int index) => $"Location {companyName[companyName.IndexOf('#')..]}:{index}";

    /// <summary>
    /// Return location type
    /// </summary>
    /// <returns></returns>
    private static LocationType GetLocationType() => (LocationType)Random.Shared.Next((int)LocationType.Office, (int)LocationType.Depot + 1);
    #endregion

}