namespace bma.Application.Transfer.SignOff.Dtos;

/// <summary>
/// DTO for creating a signature request.
/// </summary>
public class CreateSignOffRequestDto
{
    /// <summary>
    /// Gets or sets the list of participant emails for sign-off on the signoff request.
    /// </summary>
    public List<string> SignOffParticipants { get; set; } = new();

    /// <summary>
    /// Additional information about the documents.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}