using bma.Domain.Entities.RequestEntities;

namespace bma.Domain.Interfaces;
/// <summary>
/// Repository interface for managing expense requests.
/// </summary>
public interface IExpensesRequestRepository : IRepository<ExpensesRequest>
{
    /// <summary>
    /// Retrieves all expense requests for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of expense requests.</returns>
    /// /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<ExpensesRequest>> GetAllExpenseRequestsForUserAsync(string userId);

    /// <summary>
    /// Retrieves all expense requests for a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of expense requests.</returns>
    /// /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<ExpensesRequest>> GetAllExpenseRequestsForCompanyAsync(int companyId);
}
