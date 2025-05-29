using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly BmaDbContext _context;

    public ApplicationUserRepository(BmaDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The user, or null if not found.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<ApplicationUser?> GetByIdAsync(string userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Retrieves an application user by their email address.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

}

