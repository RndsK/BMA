using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bma.Domain.Entities;

/// <summary>
/// Represents a company entity with an ID, name.
/// </summary>
public class Company
{
    /// <summary>
    /// Gets or sets the unique identifier for the company.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the name of the company owner.
    /// </summary>
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description for the company owner.
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country the company is located.
    /// </summary>
    [Required]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the companies industry.
    /// </summary>
    [Required]
    public string Industry { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the official company logo.
    /// </summary>
    public string? Logo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the owner of the company.
    /// </summary>
    [Required]
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owner of the company.
    /// </summary>
    [ForeignKey("OwnerId")]
    public ApplicationUser Owner { get; set; } = null!;

}
