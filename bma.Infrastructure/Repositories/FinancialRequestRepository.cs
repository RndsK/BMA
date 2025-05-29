using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

/// <summary>
/// Repository for managing transfer requests in the database.
/// </summary>
public class FinancialRequestRepository : Repository<FinancialRequest>, IFinancialRequestRepository
{
    private readonly BmaDbContext _context;
    public FinancialRequestRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all transfer requests made by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose transfer requests are being retrieved.</param>
    /// <returns>A list of transfer requests made by the specified user.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<FinancialRequest>> GetAllFinancialRequestsForUserAsync(string userId)
    {
        return await _context.FinancialRequests
            .Where(tr => tr.UserId == userId)
            .Include(e => e.User)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all transfer requests associated with a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company whose transfer requests are being retrieved.</param>
    /// <returns>A list of transfer requests associated with the specified company.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<FinancialRequest>> GetAllFinancialRequestsForCompanyAsync(int companyId)
    {
        return await _context.FinancialRequests
            .Where(tr => tr.CompanyId == companyId)
            .Include(e => e.User)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new transfer request sign-off participant.
    /// </summary>
    /// <param name="participant">The participant to add.</param>
    public async Task CreateSignOffParticipantAsync(TransferRequestSignOffParticipant participant)
    {
        await _context.TransferRequestSignOffParticipants.AddAsync(participant);
    }
}
