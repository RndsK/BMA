namespace bma.Application.Expenses.Dtos;

/// <summary>
/// Data transfer object for creating a new expense request.
/// </summary>
public class CreateExpenseRequestDto
{
    /// <summary>
    /// The amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The currency of the expense.
    /// </summary>
    public string Currency { get; set; } = "CHF";

    /// <summary>
    /// The type of expense (e.g., Office, Travel).
    /// </summary>
    public string ExpenseType { get; set; } = "Office";

    /// <summary>
    /// The name of the associated project (optional).
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Additional comments about the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
