using System.ComponentModel.DataAnnotations;
using bma.Domain.Constants;

namespace bma.Domain.Entities.RequestEntities;

/// <summary>
/// Represents an expense request within the system.
/// </summary>
public class ExpensesRequest : Request
{
    /// <summary>
    /// Gets or sets the amount of the expense
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency of the expense.
    /// Defaults to CHF (Swiss Franc).
    /// </summary>
    [Required]
    public string Currency { get; set; } = StringDefinitions.CurrencyTypeCHF;

    /// <summary>
    /// Gets or sets the type of the expense.
    /// Examples include Office, Travel, Entertainment etc..
    /// Defaults to Office.
    /// </summary>
    [Required]
    public string ExpenseType { get; set; } = StringDefinitions.ExpenseTypeOffice;

    /// <summary>
    /// Gets or sets the name of the project associated with the expense.
    /// Optional field.
    /// </summary>
    public string? ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL or path to the attachment associated with the expense.
    /// Optional field.
    /// </summary>
    public string? Attachment { get; set; } = string.Empty;
}
