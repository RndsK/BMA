namespace bma.Application.Approvals.Dtos;
/// <summary>
/// DTO for an approval
/// </summary>
public class ApprovalDto
{
    /// <summary>
    /// Primary key for the approval
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Request status.
    /// Can be Accepted or Rejected
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the request the approval is for.
    /// </summary>
    public int? RequestId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the join request the approval is for.
    /// </summary>
    public int? JoinRequestId { get; set; }

    /// <summary>
    /// The email of the user that made the approval.
    /// </summary>
    public string ApprovedBy { get; set; } = string.Empty;
}
