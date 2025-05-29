using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

/// <summary>
/// Repository for managing expenses requests.
/// </summary>
public class ExpensesRequestRepository : Repository<ExpensesRequest>, IExpensesRequestRepository
{
    private readonly BmaDbContext _context;

    public ExpensesRequestRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all expense requests for a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a collection of <see cref="ExpensesRequest"/> for the specified company.
    /// </returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<ExpensesRequest>> GetAllExpenseRequestsForCompanyAsync(int companyId)
    {
        return await _context.Set<ExpensesRequest>()
            .Where(e => e.CompanyId == companyId)
            .Include(e => e.User)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all expense requests for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// The task result contains a collection of <see cref="ExpensesRequest"/> for the specified user.
    /// </returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<ExpensesRequest>> GetAllExpenseRequestsForUserAsync(string userId)
    {
        return await _context.Set<ExpensesRequest>()
            .Where(e => e.UserId == userId)
            .Include(e => e.User)
            .AsNoTracking()
            .ToListAsync();
    }
}

