using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

public class RoleInCompanyRepository : Repository<RoleInCompany>, IRoleInCompanyRepository
{
    private readonly BmaDbContext _context;

    public RoleInCompanyRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new role for a user in a specified company.
    /// </summary>
    /// <param name="userId">The ID of the user to assign the role to.</param>
    /// <param name="companyId">The ID of the company where the role is being assigned.</param>
    /// <param name="roleName">The name of the role to assign (e.g., Owner, Manager).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public async Task AddRoleInCompanyAsync(string userId, int companyId, string roleName)
    {
        var roleInCompany = new RoleInCompany
        {
            UserId = userId,
            CompanyId = companyId,
            Name = roleName
        };
        await _context.Set<RoleInCompany>().AddAsync(roleInCompany);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all roles associated with a specified user.
    /// </summary>
    /// <param name="userId">The ID of the user whose roles are to be retrieved.</param>
    /// <returns>An enumerable collection of <see cref="RoleInCompany"/> objects associated with the user.</returns>
    public async Task<IEnumerable<RoleInCompany>> GetByUserIdAsync(string userId)
    {
        return await _context.RoleInCompany
            .Where(r => r.UserId == userId)
            .Include(r => r.Company)
            .ToListAsync();
    }
}
