using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace bma.Infrastructure.Repositories;

public class ApplicationUserRoleRepository : Repository<IdentityRole>, IApplicationUserRoleRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserRoleRepository(BmaDbContext context, UserManager<ApplicationUser> userManager) : base(context)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Checks if a user is in a specific role.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="role">The role name.</param>
    /// <returns>True if the user is in the role, otherwise false.</returns>
    public async Task<bool> IsUserInRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    /// <summary>
    /// Adds a user to a specific role.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="role">The role name.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddUserToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
