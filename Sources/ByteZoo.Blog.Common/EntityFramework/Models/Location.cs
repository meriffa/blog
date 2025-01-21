using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteZoo.Blog.Common.EntityFramework.Models;

/// <summary>
/// Location
/// </summary>
[Table(nameof(Location)), Index(nameof(Name), IsUnique = true)]
public class Location
{

    #region Properties
    /// <summary>
    /// Location id
    /// </summary>
    [Column("LocationID")]
    public required int Id { get; set; }

    /// <summary>
    /// Location company
    /// </summary>
    [ForeignKey("CompanyID")]
    public required Company Company { get; set; }

    /// <summary>
    /// Location type
    /// </summary>
    [Column("LocationTypeID")]
    public required LocationType Type { get; set; }

    /// <summary>
    /// Location name
    /// </summary>
    [Column("LocationName"), Unicode(true), MaxLength(512)]
    public required string Name { get; set; }
    #endregion

}