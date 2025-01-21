using ByteZoo.Blog.Common.EntityFramework.Contexts;
using ByteZoo.Blog.Common.EntityFramework.Models;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Entity Framework company seed controller
/// </summary>
[Verb("Tools-EFSeedCompany", HelpText = "Entity Framework seed Company operation.")]
public class EFSeedCompanyController : EFController
{

    #region Private Members
    private int nextCompanyId;
    #endregion

    #region Properties
    /// <summary>
    /// Number of top level companies
    /// </summary>
    [Option("topLevelCompanies", Required = true, HelpText = "Number of top level companies.")]
    public int TopLevelCompanies { get; set; }

    /// <summary>
    /// Minimum number of levels per company
    /// </summary>
    [Option("levelsPerCompanyMin", Required = true, HelpText = "Minimum number of levels per company.")]
    public int LevelsPerCompanyMin { get; set; }

    /// <summary>
    /// Maximum number of levels per company
    /// </summary>
    [Option("levelsPerCompanyMax", Required = true, HelpText = "Maximum number of levels per company.")]
    public int LevelsPerCompanyMax { get; set; }

    /// <summary>
    /// Minimum number of subsidiaries per level
    /// </summary>
    [Option("subsidiariesPerLevelMin", Required = true, HelpText = "Minimum number of subsidiaries per level.")]
    public int SubsidiariesPerLevelMin { get; set; }

    /// <summary>
    /// Maximum number of subsidiaries per level
    /// </summary>
    [Option("subsidiariesPerLevelMax", Required = true, HelpText = "Maximum number of subsidiaries per level.")]
    public int SubsidiariesPerLevelMax { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        nextCompanyId = TopLevelCompanies;
        Parallel.ForEach(Enumerable.Range(1, TopLevelCompanies), companyId =>
        {
            using var context = new DatabaseContext(Type, ConnectionString);
            var levelsPerCompany = Random.Shared.Next(LevelsPerCompanyMin, LevelsPerCompanyMax + 1);
            var subsidiariesPerLevel = Random.Shared.Next(SubsidiariesPerLevelMin, SubsidiariesPerLevelMax + 1);
            CreateCompanyHierarchy(context, companyId, levelsPerCompany, subsidiariesPerLevel);
        });
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Create company hierarchy
    /// </summary>
    /// <param name="context"></param>
    /// <param name="companyId"></param>
    /// <param name="levelsPerCompany"></param>
    /// <param name="subsidiariesPerLevel"></param>
    private void CreateCompanyHierarchy(DatabaseContext context, int companyId, int levelsPerCompany, int subsidiariesPerLevel)
    {
        var company = new Company() { Id = companyId, Name = GetCompanyName(companyId) };
        context.Companies.Add(company);
        context.SaveChanges();
        var subsidiaryCount = 0;
        if (levelsPerCompany > 1)
            for (int i = 0; i < subsidiariesPerLevel; i++)
                CreateCompanySubsidiary(context, company, 2, levelsPerCompany, subsidiariesPerLevel, ref subsidiaryCount);
        displayService.WriteInformation($"Company hierarchy created (ID = {company.Id}, Name = '{company.Name}', Subsidiaries = {subsidiaryCount}).");
    }

    /// <summary>
    /// Create company subsidiary and nested companies
    /// </summary>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    /// <param name="currentLevel"></param>
    /// <param name="levelsPerCompany"></param>
    /// <param name="subsidiariesPerLevel"></param>
    /// <param name="subsidiaryCount"></param>
    private void CreateCompanySubsidiary(DatabaseContext context, Company parent, int currentLevel, int levelsPerCompany, int subsidiariesPerLevel, ref int subsidiaryCount)
    {
        var companyId = GenerateCompanyId();
        var company = new Company() { Id = companyId, Name = GetCompanyName(parent.Name, companyId), Parent = parent };
        context.Companies.Add(company);
        context.SaveChanges();
        subsidiaryCount++;
        if (currentLevel < levelsPerCompany)
            for (int i = 0; i < subsidiariesPerLevel; i++)
                CreateCompanySubsidiary(context, company, currentLevel + 1, levelsPerCompany, subsidiariesPerLevel, ref subsidiaryCount);
    }

    /// <summary>
    /// Return new company id
    /// </summary>
    /// <returns></returns>
    private int GenerateCompanyId() => Interlocked.Increment(ref nextCompanyId);

    /// <summary>
    /// Return company name
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    private static string GetCompanyName(int companyId) => $"Company #{companyId}";

    /// <summary>
    /// Return company name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="companyId"></param>
    /// <returns></returns>
    private static string GetCompanyName(string name, int companyId) => $"{name}.{companyId}";
    #endregion

}