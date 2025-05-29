using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace bma.Domain.Entities;

/// <summary>
/// Represents an application user with Identity properties and additional custom fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role of the user.
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street address of the user.
    /// </summary>
    public string? StreetAddress { get; set; }

    /// <summary>
    /// Gets or sets the city where the user resides.
    /// </summary>
    public string? City { get; set; }

    // <summary>
    /// Gets or sets the state where the user resides.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the postal code of the user's address.
    /// </summary>
    public string? PostalCode { get; set; }
}
