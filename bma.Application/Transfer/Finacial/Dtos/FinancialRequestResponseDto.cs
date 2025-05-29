using bma.Domain.Constants;
using bma.Application.Requests.Dtos;

namespace bma.Application.Transfer.Finacial.Dtos;

/// <summary>
/// DTO for transfer request response.
/// </summary>
public class FinancialRequestResponseDto : RequestDto
{

    /// <summary>
    /// Gets or sets the type of the transfer request.
    /// Example: Budget, Payment, Investment, etc.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recurrence type of the transfer.
    /// Describes if the payment is one-off, weekly, monthly, etc.
    /// </summary>
    public string RecurrenceType { get; set; } = StringDefinitions.RecurrenceTypeOneOff;

    /// <summary>
    /// Gets or sets the currency for the transfer.
    /// Example: CHF, EUR, USD.
    /// </summary>
    public string Currency { get; set; } = StringDefinitions.CurrencyTypeCHF;

    /// <summary>
    /// Gets or sets the amount to be transferred.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the source account for the transfer.
    /// </summary>
    public string TransferFrom { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination account for the transfer.
    /// </summary>
    public string TransferTo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of participant emails for sign-off on the transfer request.
    /// </summary>
    public List<string> SignOffParticipants { get; set; } = new();

    /// <summary>
    /// Gets or sets the supporting document for the transfer request.
    /// </summary>
    public string? SupportingDocument { get; set; }
}
