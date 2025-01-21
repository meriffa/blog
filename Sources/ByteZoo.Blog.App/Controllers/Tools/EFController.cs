using ByteZoo.Blog.Common.EntityFramework;
using CommandLine;

namespace ByteZoo.Blog.App.Controllers.Tools;

/// <summary>
/// Entity Framework controller
/// </summary>
public abstract class EFController : Controller
{

    #region Properties
    /// <summary>
    /// Database type
    /// </summary>
    [Option('t', "databaseType", Required = true, HelpText = "Database type.")]
    public DatabaseType Type { get; set; }

    /// <summary>
    /// Database connection string
    /// </summary>
    [Option('c', "connectionString", Required = true, HelpText = "Database connection string.")]
    public string ConnectionString { get; set; } = null!;
    #endregion

}