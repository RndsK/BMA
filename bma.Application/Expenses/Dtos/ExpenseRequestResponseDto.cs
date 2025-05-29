using bma.Application.Requests.Dtos;

namespace bma.Application.Expenses.Dtos;

/// <summary>
/// Data transfer object for the response of an expense request.
/// </summary>
public class ExpenseRequestResponseDto : RequestDto
{
    /// <summary>
    /// Gets or sets the amount requested for the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency in which the expense amount is specified.
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of expense (e.g., Travel, Office Supplies).
    /// </summary>
    public string ExpenseType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the project associated with the expense, if applicable.
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the URL or path to the attachment (e.g., receipt) associated with the expense request, if any.
    /// </summary>
    public string? Attachment { get; set; }

    /// <summary>
    /// Gets or sets the name of the user who submitted the expense request.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
