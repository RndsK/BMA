namespace bma.Domain.Constants;

/// <summary>
/// Defines commonly used string constants throughout the application.
/// </summary>
public static class StringDefinitions
{
    //Roles

    /// <summary>
    /// Represents the "Owner" role, typically assigned to the owner of a company.
    /// Users with this role have full access and administrative privileges over the company.
    /// </summary>
    public const string Owner = "Owner";

    /// <summary>
    /// Represents the "User" role, typically assigned to regular employees or users within the system.
    /// Users with this role have restricted access based on their assigned permissions.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Represents the "Manager" role, typically assigned to individuals responsible for managing teams or resources.
    /// </summary>
    public const string Manager = "Manager";

    /// <summary>
    /// Request statuses
    /// </summary>
    public const string RequestStatusPending = "Pending";
    public const string RequestStatusApproved = "Approved";
    public const string RequestStatusRejected = "Rejected";
    public const string RequestStatusCancelled = "Cancelled";

    /// <summary>
    /// Currency codes
    /// </summary>
    public const string CurrencyTypeCHF = "CHF";
    public const string CurrencyTypeGBP = "GBP";
    public const string CurrencyTypeUSD = "USD";
    public const string CurrencyTypeEUR = "EUR";
    public const string CurrencyTypeAED = "AED";

    /// <summary>
    /// Expenses related to office supplies or activities.
    /// </summary>
    public const string ExpenseTypeOffice = "Office";
    /// <summary>
    /// Expenses incurred during travel.
    /// </summary>
    public const string ExpenseTypeTravel = "Travel";
    /// <summary>
    /// Expenses for food and drink.
    /// </summary>
    public const string ExpenseTypeFoodAndDrink = "Food & Drink";
    /// <summary>
    /// Expenses for lodging or accommodation.
    /// </summary>
    public const string ExpenseTypeAccommodation = "Accommodation";
    /// <summary>
    /// Expenses related to IT and communication.
    /// </summary>
    public const string ExpenseTypeItAndComms = "IT & Comms";
    /// <summary>
    /// Expenses that do not fit into other categories.
    /// </summary>
    public const string ExpenseTypeOther = "Other";


    public const string TransferTypeBudget = "Budget";
    public const string TransferTypePayment = "Payment";
    public const string TransferTypeInvestment = "Investment";
    public const string TransferTypeInternal = "Internal";
    public const string TransferTypeDocument = "Document";
    public const string TransferTypeOther = "Other";


    public const string RecurrenceTypeOneOff = "One-off";
    public const string RecurrenceTypeWeekly = "Weekly";
    public const string RecurrenceTypeMonthly = "Monthly";
    public const string RecurrenceTypeQuarterly = "Quarterly";
    public const string RecurrenceTypeAnnually = "Annually";


    public const string SignOffStatusSigned = "Signed";
    public const string SignOffStatusNotSigned = "Not Signed";
}
