using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly IApplicationUserRoleRepository _roleRepository;
        private readonly BmaDbContext _context;
        private readonly IRoleInCompanyRepository _roleInCompanyRepository;
        public CompanyRepository(BmaDbContext context,
            IApplicationUserRepository userRepository,
            IApplicationUserRoleRepository roleRepository,
            IRoleInCompanyRepository roleInCompanyRepository) : base(context)
        {
            _context = context;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleInCompanyRepository = roleInCompanyRepository;
        }

        /// <summary>
        /// Creates a new company and assigns the user as the owner.
        /// </summary>
        /// <param name="userId">The ID of the user creating the company.</param>
        /// <param name="companyName">The name of the company.</param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        public async Task CreateCompanyAsync(string userId, Company company)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            company.OwnerName = user.Name;
            company.OwnerId = user.Id;
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            if (user != null)
            {
                user.Role = StringDefinitions.Owner;
                _userRepository.Update(user);
                await _context.SaveChangesAsync();

                if (!await _roleRepository.IsUserInRole(userId, StringDefinitions.Owner))
                {
                    await _roleRepository.AddUserToRoleAsync(userId, StringDefinitions.Owner);
                    await _context.SaveChangesAsync();
                }

                await _roleInCompanyRepository.AddRoleInCompanyAsync(userId, company.Id, StringDefinitions.Owner);
                await _context.SaveChangesAsync();

            }

        }


        /// <summary>
        /// Retrieves a list of companies.
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _context.Companies
                .Include(uc => uc.Owner)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a list of company IDs associated with a specific user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose company associations are to be retrieved.</param>
        /// <returns>A list of company IDs associated with the user.</returns>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<List<int>> GetAllByUserIdAsync(string userId)
        {
            return await _context.RoleInCompany
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.CompanyId)
                .ToListAsync();
        }
    }
}