using bma.Domain.Entities;

namespace bma.Domain.Interfaces;

/// <summary>
/// Repository interface for managing roles within companies.
/// </summary>
/// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
public interface IRoleInCompanyRepository : IRepository<RoleInCompany>
{
    /// <summary>
    /// Adds a new role for a user in a specified company.
    /// </summary>
    /// <param name="userId">The ID of the user to assign the role to.</param>
    /// <param name="companyId">The ID of the company where the role is being assigned.</param>
    /// <param name="roleName">The name of the role to assign (e.g., Owner, Manager).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task AddRoleInCompanyAsync(string userId, int companyId, string roleName);

    /// <summary>
    /// Retrieves all roles associated with a specified user.
    /// </summary>
    /// <param name="userId">The ID of the user whose roles are to be retrieved.</param>
    /// <returns>An enumerable collection of <see cref="RoleInCompany"/> objects associated with the user.</returns>
    Task<IEnumerable<RoleInCompany>> GetByUserIdAsync(string userId);
}
