using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteZoo.Blog.Common.EntityFramework.Models;

/// <summary>
/// Company
/// </summary>
[Table(nameof(Company)), Index(nameof(Name), IsUnique = true)]
public class Company
{

    #region Properties
    /// <summary>
    /// Company id
    /// </summary>
    [Column("CompanyID"), DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }

    /// <summary>
    /// Parent company
    /// </summary>
    [ForeignKey("ParentCompanyID")]
    public Company? Parent { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    [Column("CompanyName"), Unicode(true), MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    /// Company subsidiaries
    /// </summary>
    public List<Company> Subsidiaries { get; } = [];

    /// <summary>
    /// Company locations
    /// </summary>
    public List<Location> Locations { get; } = [];
    #endregion

}