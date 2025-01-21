using Microsoft.EntityFrameworkCore.Design;

namespace ByteZoo.Blog.Common.EntityFramework.Contexts;

/// <summary>
/// Database context factory
/// </summary>
public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{

    #region Public Methods
    /// <summary>
    /// Create database context
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public DatabaseContext CreateDbContext(string[] args)
    {
        if (args == null || args.Length == 0 || string.IsNullOrEmpty(args[0]))
            throw new("Database type is not specified.");
        if (args.Length == 1 || string.IsNullOrEmpty(args[1]))
            throw new("Database connection is not specified.");
        return new DatabaseContext(Enum.Parse<DatabaseType>(args[0]), args[1]);
    }
    #endregion

}