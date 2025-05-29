using bma.Domain.Constants;

namespace bma.Application.JoinRequests.Dtos;
/// <summary>
/// DTO for an approval
/// </summary>
public class JoinRequestDto
{
    /// <summary>
    /// Gets or sets the primary key for JoinRequest
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Request status.
    /// Can be Pending, Approved, Rejected, and Cancelled.
    /// </summary>
    public string Status { get; set; } = StringDefinitions.RequestStatusPending;

    /// <summary>
    /// Date and time at what the request got accepted, if accepted
    /// </summary>
    public DateOnly? AcceptanceDate { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier linking the request to a company.
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Gets or sets foreign key to the user that made the request
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}