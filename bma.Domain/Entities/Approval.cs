using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bma.Domain.Entities.RequestEntities;

namespace bma.Domain.Entities;
/// <summary>
/// Class for approvals
/// </summary>
public class Approval
{
    /// <summary>
    /// Primary key for the approval
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Request status.
    /// Can be Accepted or Rejected
    /// </summary>
    [Required]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the request the approval is for.
    /// </summary>
    public int? RequestId { get; set; }

    /// <summary>
    /// Gets or sets the joint request within which the approval exists.
    /// </summary>
    [ForeignKey("JoinRequestId")]
    public JoinRequest? JoinRequest { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the join request the approval is for.
    /// </summary>
    public int? JoinRequestId { get; set; }

    /// <summary>
    /// Gets or sets the request within which the approval exists.
    /// </summary>
    [ForeignKey("RequestId")]
    public Request? Request { get; set; } = null!;

    /// <summary>
    /// The email of the user that made the approval.
    /// </summary>
    [Required]
    public string ApprovedBy { get; set; } = string.Empty;
}