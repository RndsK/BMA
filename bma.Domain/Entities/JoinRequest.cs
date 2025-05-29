using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bma.Domain.Constants;

namespace bma.Domain.Entities;
/// <summary>
/// A class for managing the requests to join a company (IS NOT PART OF THE WITHIN COMPANY REQUESTS)
/// </summary>
public class JoinRequest
{
    /// <summary>
    /// Gets or sets the primary key for JoinRequest
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Request status.
    /// Can be Pending, Approved, Rejected, and Cancelled.
    /// </summary>
    [Required]
    public string Status { get; set; } = StringDefinitions.RequestStatusPending;

    /// <summary>
    /// Date and time at what the request got accepted, if accepted
    /// </summary>
    public DateOnly? AcceptanceDate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier linking the request to a company.
    /// </summary>
    [Required]
    public int CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the company associated with the request.
    /// </summary>
    [ForeignKey("CompanyId")]
    public Company Company { get; set; } = null!;

    /// <summary>
    /// Gets or sets foreign key to the user that made the request
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user that created the request.
    /// </summary>
    [ForeignKey("UserId")]
    public ApplicationUser Applicant { get; set; } = null!;
}
