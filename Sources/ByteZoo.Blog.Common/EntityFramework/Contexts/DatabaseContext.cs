using ByteZoo.Blog.Common.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace ByteZoo.Blog.Common.EntityFramework.Contexts;

/// <summary>
/// Database context
/// </summary>
/// <param name="type"></param>
/// <param name="connectionString"></param>
public class DatabaseContext(DatabaseType type, string connectionString) : DbContext
{

    #region Properties
    /// <summary>
    /// Database context companies
    /// </summary>
    public DbSet<Company> Companies { get; set; }

    /// <summary>
    /// Database context locations
    /// </summary>
    public DbSet<Location> Locations { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Configure database context
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (type)
        {
            case DatabaseType.SQLite:
                optionsBuilder.UseSqlite(connectionString);
                break;
            case DatabaseType.PostgreSQL:
                optionsBuilder.UseNpgsql(connectionString);
                break;
            case DatabaseType.MySQL:
                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 3)));
                break;
            case DatabaseType.SQLServer:
                optionsBuilder.UseSqlServer(connectionString);
                break;
            default:
                throw new($"Database type {type} is not supported.");
        }
    }
    #endregion

}