using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;

namespace bma.Domain.Interfaces;

/// <summary>
/// Repository interface for managing signoff requests.
/// </summary>
public interface ISignOffRequestRepository : IRepository<SignOffRequest>
{
    /// <summary>
    /// Retrieves all signoff requests made by a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose signoff requests are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of signoff requests associated with the specified user.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<SignOffRequest>> GetAllSignOffRequestsForUserAsync(string userId);

    /// <summary>
    /// Retrieves all signoff requests associated with a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company whose signoff requests are to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of signoff requests associated with the specified company.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<SignOffRequest>> GetAllSignOffRequestsForCompanyAsync(int companyId);

    /// <summary>
    /// Adds a new transfer request sign-off participant.
    /// </summary>
    /// <param name="participant">The participant to add.</param>
    Task CreateSignOffParticipantAsync(TransferRequestSignOffParticipant participant);
}
