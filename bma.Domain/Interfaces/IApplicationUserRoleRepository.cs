using Microsoft.AspNetCore.Identity;

namespace bma.Domain.Interfaces;

public interface IApplicationUserRoleRepository : IRepository<IdentityRole>
{
    Task<bool> IsUserInRole(string userId, string role);
    Task AddUserToRoleAsync(string userId, string role);
}
