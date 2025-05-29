using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Exceptions;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories
{
    public class JoinRequestRepository : Repository<JoinRequest>, IJoinRequestRepository
    {
        private readonly BmaDbContext _context;

        public JoinRequestRepository(BmaDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new company and assigns the user as the owner.
        /// </summary>
        /// <param name="userId">The ID of the user creating the company.</param>
        /// <param name="companyId">The id of the company.</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="DuplicateJoinRequestException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        public async Task CreateJoinRequestAsync(string userId, int companyId)
        {
            var existingRequest = await _context.JoinRequests
                .FirstOrDefaultAsync(jr => jr.UserId == userId && jr.CompanyId == companyId);

            if (existingRequest != null)
            {
                throw new DuplicateJoinRequestException(userId, companyId);
            }

            var JoinRequest = new JoinRequest
            {
                UserId = userId,
                CompanyId = companyId,
                Status = StringDefinitions.RequestStatusPending
            };

            await _context.Set<JoinRequest>().AddAsync(JoinRequest);

            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Get join request of user by companyId
        /// </summary>
        /// <param name="userId">The ID of the user to search for.</param>
        /// /// <param name="companyId">The ID of the company to search within.</param>
        /// <returns>The join request that was approved for the user to be part of the company, or null if user is not part of this company.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<JoinRequest?> GetJoinRequestForUserByCompanyAsync(string userId, int companyId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            return await _context.JoinRequests
                .Where(jr => jr.UserId == userId && jr.AcceptanceDate.HasValue && jr.CompanyId == companyId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a list of join requests by the company id.
        /// </summary>
        /// <param name="companyId">The ID of the company whose join requests is getting checked.</param>
        /// <returns>A list of join requests associated with the company.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<IEnumerable<JoinRequest>> GetAllJoinRequestsByCompanyIdAsync(int companyId)
        {
            return await _context.JoinRequests
                .Where(jr => jr.CompanyId == companyId && jr.Status == StringDefinitions.RequestStatusPending)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a list of join requests by the company and user id's.
        /// </summary>
        /// <param name="companyId">The ID of the company whose join requests is getting checked.</param>
        /// <param name="userId">The ID of the user whose join requests is getting checked.</param>
        /// <returns>A list of join requests associated with the company and user.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<IEnumerable<JoinRequest>> GetAllJoinRequestsByUserAndCompanyAsync(int companyId, string userId)
        {
            return await _context.JoinRequests
                .Where(jr => jr.CompanyId == companyId && jr.UserId == userId)
                .ToListAsync();
        }
        
    }
}