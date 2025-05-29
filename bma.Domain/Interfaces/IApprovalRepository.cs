using bma.Domain.Entities;

namespace bma.Domain.Interfaces;

/// <summary>
/// Repository interface for managing roles in companies.
/// </summary>
public interface IApprovalRepository : IRepository<Approval>
{
    /// <summary>
    /// Creates a new approval for a specified request.
    /// </summary>
    /// <param name="requestId">The ID of the request to be approved.</param>
    /// <param name="approvedBy">The email or identifier of the user who approved the request.</param>
    /// <param name="status">The status of the approval (e.g., Approved, Rejected).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task CreateApprovalForRequest(int requestId, string approvedBy, string status);

    /// <summary>
    /// Creates a new approval for a specified join request.
    /// </summary>
    /// <param name="joinRequestId">The ID of the join request to be approved.</param>
    /// <param name="approvedBy">The email or identifier of the user who approved the join request.</param>
    /// <param name="status">The status of the approval (e.g., Approved, Rejected).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task CreateApprovalForJoinRequest(int joinRequestId, string approvedBy, string status);

    /// <summary>
    /// Retrieves all approvals associated with a specified user.
    /// </summary>
    /// <param name="userId">The ID of the user whose approvals are to be retrieved.</param>
    /// <returns>An enumerable collection of approvals associated with the user.</returns>
    Task<IEnumerable<Approval>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Retrieves all approvals associated with a specified company.
    /// </summary>
    /// <param name="companyId">The ID of the company whose approvals are to be retrieved.</param>
    /// <returns>An enumerable collection of approvals associated with the company.</returns>
    Task<IEnumerable<Approval>> GetByCompanyIdAsync(int companyId);
}