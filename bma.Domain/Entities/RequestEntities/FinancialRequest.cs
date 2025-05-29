using System.ComponentModel.DataAnnotations;
using bma.Domain.Constants;

namespace bma.Domain.Entities.RequestEntities;

/// <summary>
/// Represents a transfer request within the company.
/// </summary>
public class FinancialRequest : Request
{
    /// <summary>
    /// The type of transfer request.
    /// Can be Budget, Payment, Investment, etc.
    /// </summary>
    [Required]
    public string Type { get; set; } = StringDefinitions.TransferTypeInternal;

    /// <summary>
    /// The recurrence of the transfer.
    /// Describes if the payment needs to be a one off, weekly, monthly etc.
    /// </summary>
    [Required]
    public string RecurrenceType { get; set; } = StringDefinitions.RecurrenceTypeOneOff;

    /// <summary>
    /// The currency in which the value needs to be assigned
    /// </summary>
    [Required]
    public string Currency { get; set; } = StringDefinitions.CurrencyTypeCHF;

    /// <summary>
    /// A number value for the cost
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// The account the money amount should be transferred from
    /// </summary>
    [Required]
    public string TransferFrom { get; set; } = string.Empty;

    /// <summary>
    /// The account the money amount should be transferred to
    /// </summary>
    [Required]
    public string TransferTo { get; set; } = string.Empty;

    /// <summary>
    /// Any additional documentation for the transfer
    /// </summary>
    public string? SupportingDocument { get; set; }

    /// <summary>
    /// Company employee or employees, that is responsible for accepting or denying the request
    /// </summary>
    [Required]
    public List<string> SignOffParticipants { get; set; } = new();
}
