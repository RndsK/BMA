using bma.Domain.Entities.RequestEntities;

namespace bma.Domain.Interfaces;

/// <summary>
/// Repository interface for managing requests.
/// </summary>
/// <exception cref="OperationCanceledException"></exception>
public interface IRequestRepository : IRepository<Request>
{
    Task<(IEnumerable<(Request Request, string Discriminator)> Requests, int TotalCount)> GetPagedAndFilteredPendingRequestsAsync(int companyId, string? searchQuery, int pageNumber, int pageSize );
}