using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bma.Domain.Constants;

namespace bma.Domain.Entities.RequestEntities;
/// <summary>
/// Parent class for all requests within the company
/// </summary>
public class Request
{
    /// <summary>
    /// Primary key for the request
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the request status.
    /// Can be Accepted, Rejected and Cancelled
    /// </summary>
    [Required]
    public string Status { get; set; } = StringDefinitions.RequestStatusPending;

    /// <summary>
    /// Gets or sets a description for the request, will include different information
    /// about the request depending on what kind of request it is
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the company the request exists within.
    /// </summary>
    [Required]
    public int CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the company within which the request exists.
    /// </summary>
    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the user that made the request.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user that created the request.
    /// </summary>
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

}
