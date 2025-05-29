using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace bma.UnitTests.RepositoryTests;

public class RoleInCompanyRepositoryTests : IDisposable
{
    private readonly DbContextOptions<BmaDbContext> _options;
    private readonly BmaDbContext _context;

    public RoleInCompanyRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: "RoleInCompanyTestDb")
            .Options;

        _context = new BmaDbContext(_options);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Fact]
    public async Task AddRoleInCompanyAsync_WithValidData_AddsRoleInCompany()
    {
        // Arrange
        var repository = new RoleInCompanyRepository(_context);

        var user = new ApplicationUser
        {
            Id = "user1",
            UserName = "TestUser",
            Email = "testuser@example.com",
            Role = StringDefinitions.User // Assuming you have a role property
        };

        var companyId = 1;
        var roleName = "Admin";

        var company = new Company { Id = companyId, Name = "Test Company", OwnerId = user.Id };
        _context.Companies.Add(company);
        _context.Users.Add(new ApplicationUser { Id = user.Id, Email = "user1@example.com" });
        await _context.SaveChangesAsync();

        // Act
        await repository.AddRoleInCompanyAsync(user.Id, company.Id, roleName);
        await _context.SaveChangesAsync();

        // Assert
        var roleInCompany = await _context.RoleInCompany.FirstOrDefaultAsync(r => r.UserId == user.Id && r.CompanyId == companyId);
        roleInCompany.Should().NotBeNull();
        roleInCompany!.Name.Should().Be(roleName);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidUserId_ReturnsRolesInCompanies()
    {
        // Arrange
        var repository = new RoleInCompanyRepository(_context);

        var userId = "user2";
        var companyId1 = 1;
        var companyId2 = 2;

        _context.Companies.AddRange(
            new Company { Id = companyId1, Name = "Company A", OwnerId = "owner1" },
            new Company { Id = companyId2, Name = "Company B", OwnerId = "owner2" }
        );

        _context.Users.Add(new ApplicationUser { Id = userId, Email = "user2@example.com" });

        _context.RoleInCompany.AddRange(
            new RoleInCompany { UserId = userId, CompanyId = companyId1, Name = "Employee" },
            new RoleInCompany { UserId = userId, CompanyId = companyId2, Name = "Manager" }
        );

        await _context.SaveChangesAsync();

        // Act
        var rolesInCompanies = await repository.GetByUserIdAsync(userId);

        // Assert
        rolesInCompanies.Should().HaveCount(2);
        rolesInCompanies.Should().ContainSingle(r => r.CompanyId == companyId1 && r.Name == "Employee");
        rolesInCompanies.Should().ContainSingle(r => r.CompanyId == companyId2 && r.Name == "Manager");
    }

    [Fact]
    public async Task GetByUserIdAsync_WithInvalidUserId_ReturnsEmpty()
    {
        // Arrange
        var repository = new RoleInCompanyRepository(_context);

        var userId = "nonexistent";

        // Act
        var rolesInCompanies = await repository.GetByUserIdAsync(userId);

        // Assert
        rolesInCompanies.Should().BeEmpty();
    }
}
