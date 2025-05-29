using System.Linq.Expressions;
using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories
{
    public class OvertimeRequestRepository : Repository<OvertimeRequest>, IOvertimeRequestRepository
    {
        private readonly BmaDbContext _context;
        public OvertimeRequestRepository(BmaDbContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves all overtime requests for a specific company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a collection of <see cref="OvertimeRequest"/> for the specified company.
        /// </returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<IEnumerable<OvertimeRequest>> GetAllOvertimeRequestsForCompanyAsync(int companyId)
        {
            return await _context.Set<OvertimeRequest>()
                .Include(e => e.User)
                .Where(e => e.CompanyId == companyId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all overtime requests for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains a collection of <see cref="OvertimeRequest"/> for the specified user.
        /// </returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<IEnumerable<OvertimeRequest>> GetAllOvertimeRequestsForUserAsync(string userId)
        {
            return await _context.Set<OvertimeRequest>()
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
