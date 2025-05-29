using bma.Domain.Constants;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

/// <summary>
/// Repository for managing transfer requests in the database.
/// </summary>
public class RequestRepository : Repository<Request>, IRequestRepository
{
    private readonly BmaDbContext _context;
    public RequestRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated and optionally filtered list of pending requests.
    /// </summary>
    /// <param name="searchQuery">
    /// An optional search term to filter requests by type. 
    /// If null or empty, no filtering is applied.
    /// </param>
    /// <param name="pageNumber">
    /// The page number for the paginated results. Must be greater than or equal to 1.
    /// </param>
    /// <param name="pageSize">
    /// The number of companies to include per page. Must be greater than or equal to 1.
    /// </param>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<(IEnumerable<(Request Request, string Discriminator)> Requests, int TotalCount)> GetPagedAndFilteredPendingRequestsAsync(
        int companyId, string? searchQuery, int pageNumber, int pageSize)
    {
        var query = _context.Requests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(r => EF.Property<string>(r, "Discriminator").Contains(searchQuery));
        }

        var totalCount = await query
            .Where(r => r.Status == StringDefinitions.RequestStatusPending)
            .CountAsync();

        var requests = await query
            .Where(r => r.Status == StringDefinitions.RequestStatusPending)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(c => c.User)
            .AsNoTracking()
            .Select(r => new
            {
                Request = r,
                Discriminator = EF.Property<string>(r, "Discriminator")
            })
            .ToListAsync();

        var result = requests.Select(r => (r.Request, r.Discriminator));

        return (result, totalCount);

    }
}