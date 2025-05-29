using bma.Domain.Entities.RequestEntities;

namespace bma.Domain.Interfaces;

public interface IOvertimeRequestRepository : IRepository<OvertimeRequest>
{
    /// <summary>
    /// Retrieves all overtime requests for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of overtime requests.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<OvertimeRequest>> GetAllOvertimeRequestsForUserAsync(string userId);

    /// <summary>
    /// Retrieves all overtime requests for a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of overtime requests.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<OvertimeRequest>> GetAllOvertimeRequestsForCompanyAsync(int companyId);
}