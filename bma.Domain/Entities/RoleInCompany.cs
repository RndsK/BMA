using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bma.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between users and companies.
/// </summary>
public class RoleInCompany
{
    /// <summary>
    /// Gets or sets the primary key for role in company
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// The name of the role within the company for the user
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user in the relationship.
    /// </summary>
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the company.
    /// </summary>
    [Required]
    public int CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the company in the relationship.
    /// </summary>
    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = null!;
}