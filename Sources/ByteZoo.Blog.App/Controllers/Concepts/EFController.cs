using ByteZoo.Blog.Common.EntityFramework.Contexts;
using ByteZoo.Blog.Common.EntityFramework.Models;
using ByteZoo.Blog.Common.Services;
using CommandLine;
using Microsoft.EntityFrameworkCore;

namespace ByteZoo.Blog.App.Controllers.Concepts;

/// <summary>
/// Entity Framework controller
/// </summary>
[Verb("Concepts-EF", HelpText = "Entity Framework operation.")]
public class EFController : Tools.EFController
{

    #region Constants
    private const int MB = 1024 * 1024;
    #endregion

    #region Properties
    /// <summary>
    /// Database type
    /// </summary>
    [Option('i', "companyId", Required = true, HelpText = "Company ID.")]
    public int CompanyId { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        var totalSize = 128 * MB;
        var enabled = GC.TryStartNoGCRegion(totalSize);
        if (enabled)
        {
            displayService.WriteInformation($"No GC region started (Size = {totalSize / MB} MB).");
            GC.RegisterNoGCRegionCallback(totalSize, () => displayService.WriteInformation($"No GC region allocation reached {totalSize / MB} MB."));
        }
        try
        {
            using var context = new DatabaseContext(Type, ConnectionString);
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var company = context.Companies.Include(i => i.Subsidiaries).Include(i => i.Locations).First(i => i.Id == CompanyId);
            int subsidiaryCount = company.Subsidiaries.Count;
            int locationCount = company.Locations.Count;
            LoadSubsidiaries(context, company, ref subsidiaryCount, ref locationCount);
            displayService.WriteInformation($"Company hierarchy loaded (Company ID = {company.Id}, Subsidiaries = {subsidiaryCount}, Locations = {locationCount}).");
            UpdateCompany(company);
            displayService.WriteInformation($"Company hierarchy updated (Company ID = {company.Id}, Subsidiaries = {subsidiaryCount}, Locations = {locationCount}).");
            displayService.Wait();
        }
        finally
        {
            if (enabled)
            {
                GC.EndNoGCRegion();
                displayService.WriteInformation($"No GC region ended (Size = {totalSize / MB} MB).");
            }
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Load company subsidiaries
    /// </summary>
    /// <param name="context"></param>
    /// <param name="company"></param>
    /// <param name="subsidiaryCount"></param>
    /// <param name="locationCount"></param>
    private static void LoadSubsidiaries(DatabaseContext context, Company company, ref int subsidiaryCount, ref int locationCount)
    {
        foreach (var subsidiary in company.Subsidiaries)
        {
            context.Entry(subsidiary).Collection(i => i.Subsidiaries).Load();
            context.Entry(subsidiary).Collection(i => i.Locations).Load();
            subsidiaryCount += subsidiary.Subsidiaries.Count;
            locationCount += subsidiary.Locations.Count;
            LoadSubsidiaries(context, subsidiary, ref subsidiaryCount, ref locationCount);
        }
    }

    /// <summary>
    /// Update company
    /// </summary>
    /// <param name="company"></param>
    private static void UpdateCompany(Company company)
    {
        company.Name = $"{company.Name} ({DateTime.Now.ToString(DisplayService.DATE_FORMAT)})";
        foreach (var location in company.Locations)
            location.Name = $"{location.Name} ({DateTime.Now.ToString(DisplayService.DATE_FORMAT)})";
        foreach (var subsidiary in company.Subsidiaries)
            UpdateCompany(subsidiary);
    }
    #endregion

}