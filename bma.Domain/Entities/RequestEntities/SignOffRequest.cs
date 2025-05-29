using System.ComponentModel.DataAnnotations;

namespace bma.Domain.Entities.RequestEntities;

/// <summary>
/// Represents a signature request within the company.
/// </summary>
public class SignOffRequest : Request
{
    /// <summary>
    /// Gets or sets the files to be signed 
    /// </summary>
    public string? SupportingDocument { get; set; } = string.Empty;

    /// <summary>
    /// Company employee or employees, that is responsible for accepting or denying the request
    /// </summary>
    [Required]
    public List<string> SignOffParticipants { get; set; } = new();
}
