using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bma.Domain.Constants;
using bma.Domain.Entities.RequestEntities;

namespace bma.Domain.Entities;
/// <summary>
/// Represents the many-to-many relationship between users and requests.
/// </summary>
public class TransferRequestSignOffParticipant
{
    /// <summary>
    /// Gets or sets the primary key for sign off participant connection
    /// </summary>
    [Key]
    public int Id { get; set; }
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
    /// Gets or sets the ID of the request.
    /// </summary>
    [Required]
    public int RequestId { get; set; }

    /// <summary>
    /// Gets or sets the request in the relationship.
    /// </summary>
    [ForeignKey("RequestId")]
    public Request Request { get; set; } = null!;

    /// <summary>
    /// Gets or sets the status of the sign off.
    /// </summary>
    [Required]
    public string Status { get; set; } = StringDefinitions.SignOffStatusNotSigned;
}
