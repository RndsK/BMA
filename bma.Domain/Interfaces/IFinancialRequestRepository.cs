using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using Microsoft.EntityFrameworkCore;

namespace bma.Domain.Interfaces;

/// <summary>
/// Repository interface for managing transfer requests.
/// </summary>
public interface IFinancialRequestRepository : IRepository<FinancialRequest>
{
    /// <summary>
    /// Retrieves all transfer requests made by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose transfer requests are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of transfer requests associated with the specified user.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<FinancialRequest>> GetAllFinancialRequestsForUserAsync(string userId);

    /// <summary>
    /// Retrieves all transfer requests associated with a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company whose transfer requests are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of transfer requests associated with the specified company.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<FinancialRequest>> GetAllFinancialRequestsForCompanyAsync(int companyId);

    /// <summary>
    /// Adds a new transfer request sign-off participant.
    /// </summary>
    /// <param name="participant">The participant to add.</param>
    Task CreateSignOffParticipantAsync(TransferRequestSignOffParticipant participant);
}
