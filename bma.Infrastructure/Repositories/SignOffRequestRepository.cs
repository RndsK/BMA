using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

/// <summary>
/// Repository for managing transfer requests in the database.
/// </summary>
public class SignOffRequestRepository : Repository<SignOffRequest>, ISignOffRequestRepository
{
    private readonly BmaDbContext _context;
    public SignOffRequestRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all transfer requests made by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose transfer requests are being retrieved.</param>
    /// <returns>A list of transfer requests made by the specified user.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<SignOffRequest>> GetAllSignOffRequestsForUserAsync(string userId)
    {
        return await _context.SignOffRequests
            .Where(tr => tr.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all SignOff requests associated with a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company whose SignOff requests are being retrieved.</param>
    /// <returns>A list of SignOff requests associated with the specified company.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<SignOffRequest>> GetAllSignOffRequestsForCompanyAsync(int companyId)
    {
        return await _context.SignOffRequests
            .Where(tr => tr.CompanyId == companyId)
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
