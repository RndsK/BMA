using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

public class ApprovalRepository : Repository<Approval>, IApprovalRepository
{
    private readonly BmaDbContext _context;

    public ApprovalRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new approval for a specified request.
    /// </summary>
    /// <param name="requestId">The ID of the request to be approved.</param>
    /// <param name="approvedBy">The email or identifier of the user who approved the request.</param>
    /// <param name="status">The status of the approval (e.g., Approved, Rejected).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public async Task CreateApprovalForRequest(int requestId, string approvedBy, string status)
    {
        var approval = new Approval
        {
            RequestId = requestId,
            ApprovedBy = approvedBy,
            Status = status
        };
        await _context.Set<Approval>().AddAsync(approval);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new approval for a specified join request.
    /// </summary>
    /// <param name="joinRequestId">The ID of the join request to be approved.</param>
    /// <param name="approvedBy">The email or identifier of the user who approved the join request.</param>
    /// <param name="status">The status of the approval (e.g., Approved, Rejected).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public async Task CreateApprovalForJoinRequest(int joinRequestId, string approvedBy, string status)
    {
        var approval = new Approval
        {
            JoinRequestId = joinRequestId,
            ApprovedBy = approvedBy,
            Status = status
        };
        await _context.Set<Approval>().AddAsync(approval);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all approvals associated with a specified user.
    /// </summary>
    /// <param name="userId">The ID of the user whose approvals are to be retrieved.</param>
    /// <returns>An enumerable collection of approvals associated with the user.</returns>
    public async Task<IEnumerable<Approval>> GetByUserIdAsync(string userId)
    {
        return await _context.Approvals
            .Include(a => a.Request)
            .Include(a => a.JoinRequest)
            .Where(a => (a.Request != null && a.Request.UserId == userId) ||
                        (a.JoinRequest != null && a.JoinRequest.UserId == userId))
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all approvals associated with a specified company.
    /// </summary>
    /// <param name="companyId">The ID of the company whose approvals are to be retrieved.</param>
    /// <returns>An enumerable collection of approvals associated with the company.</returns>
    public async Task<IEnumerable<Approval>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.Approvals
            .Include(a => a.Request)
            .Include(a => a.JoinRequest)
            .Where(a =>
                (a.Request != null && a.Request.CompanyId == companyId) ||
                (a.JoinRequest != null && a.JoinRequest.CompanyId == companyId))
            .ToListAsync();
    }
}