using bma.Application.Requests.Dtos;

namespace bma.Application.Transfer.SignOff.Dtos;

/// <summary>
/// DTO for retrieving an overtime request.
/// </summary>
public class SignOffRequestResponseDto : RequestDto
{
    /// <summary>
    /// Files to be signed 
    /// </summary>
    public string? SupportingDocument { get; set; }

    /// <summary>
    /// Gets or sets the list of participant emails for sign-off on the transfer request.
    /// </summary>
    public List<string> SignOffParticipants { get; set; } = new();
}