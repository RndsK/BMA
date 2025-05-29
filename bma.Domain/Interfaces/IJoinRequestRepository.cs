using bma.Domain.Entities;
using bma.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace bma.Domain.Interfaces;

public interface IJoinRequestRepository : IRepository<JoinRequest>
{
    /// <summary>
    /// Create a new join request and assign the proper user to it and assign it to the company
    /// </summary>
    /// <param name="userId">The ID of the user creating the join request.</param>
    /// <param name="companyId">The id of the company.</param>
    /// /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="DuplicateJoinRequestException"></exception>
    /// <exception cref="DbUpdateException"></exception>
    Task CreateJoinRequestAsync(string userId, int companyId);

    /// <summary>
    /// Get join request of user by companyId
    /// </summary>
    /// <param name="userId">The ID of the user to search for.</param>
    /// <param name="companyId">The ID of the company to search within.</param>
    /// <returns>The join request that was approved for the user to be part of the company, or null if user is not part of this company.</returns>
    /// /// <exception cref="ArgumentException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<JoinRequest?> GetJoinRequestForUserByCompanyAsync(string userId, int companyId);

    /// <summary>
    /// Retrieves a list of join requests by the company id.
    /// </summary>
    /// <param name="companyId">The ID of the company whose join requests is getting checked.</param>
    /// <returns>A list of join requests associated with the company.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<JoinRequest>> GetAllJoinRequestsByCompanyIdAsync(int companyId);

    /// <summary>
    /// Retrieves a list of join requests by the company and user id's.
    /// </summary>
    /// <param name="companyId">The ID of the company whose join requests is getting checked.</param>
    /// <param name="userId">The ID of the user whose join requests is getting checked.</param>
    /// <returns>A list of join requests associated with the company and user.</returns>
    /// /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<JoinRequest>> GetAllJoinRequestsByUserAndCompanyAsync(int companyId, string userId);
}
