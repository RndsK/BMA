using bma.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace bma.Domain.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    /// <summary>
    /// Creates a new company and assigns the user as the owner.
    /// </summary>
    /// <param name="userId">The ID of the user creating the company.</param>
    /// <param name="companyDto">The name of the company.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// /// <exception cref="DbUpdateException"></exception>
    Task CreateCompanyAsync(string userId, Company company);

    /// <summary>
    /// Retrieves a list of companies.
    /// </summary>
    /// <returns>
    /// Am enumerable containing the paginated list of companies.
    /// </returns>
    /// /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<Company>> GetAllCompaniesAsync();

    /// <summary>
    /// Retrieves a list of company IDs associated with a specific user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose company associations are to be retrieved.</param>
    /// <returns>A list of company IDs associated with the user.</returns>
    /// /// <exception cref="OperationCanceledException"></exception>
    Task<List<int>> GetAllByUserIdAsync(string userId);
}