using bma.Application.Companies.Dtos;
using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace bma.UnitTests.RepositoryTests;

public class CompanyRepositoryTests : IDisposable
{
    private readonly Mock<IApplicationUserRepository> _userRepositoryMock;
    private readonly Mock<IApplicationUserRoleRepository> _roleRepositoryMock;
    private readonly Mock<IRoleInCompanyRepository> _roleInCompanyRepositoryMock;
    private readonly BmaDbContext _context;
    private readonly CompanyRepository _companyRepository;

    public CompanyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BmaDbContext(options);
        _userRepositoryMock = new Mock<IApplicationUserRepository>();
        _roleRepositoryMock = new Mock<IApplicationUserRoleRepository>();
        _roleInCompanyRepositoryMock = new Mock<IRoleInCompanyRepository>();

        _companyRepository = new CompanyRepository(
            _context,
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _roleInCompanyRepositoryMock.Object
        );
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateCompany_WithValidData_SavesCompanyAndAssignsOwner()
    {
        // Arrange
        var userId = "user123";
        var createCompany = new Company
        {
            Name = "Test Company",
            Country = "USA",
            Industry = "Technology",
            Description = "A tech company",
            Logo = "https://example.com/logo.png"
        };

        var user = new ApplicationUser { Id = userId, Role = StringDefinitions.User };

        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
        _roleRepositoryMock.Setup(repo => repo.IsUserInRole(userId, StringDefinitions.Owner)).ReturnsAsync(false);

        // Act
        await _companyRepository.CreateCompanyAsync(userId, createCompany);

        // Assert
        var createdCompany = await _context.Companies.FirstOrDefaultAsync();
        createdCompany.Should().NotBeNull();
        createdCompany.Name.Should().Be(createCompany.Name);
        createdCompany.Country.Should().Be(createCompany.Country);
        createdCompany.Industry.Should().Be(createCompany.Industry);
        createdCompany.Description.Should().Be(createCompany.Description);
        createdCompany.Logo.Should().Be(createCompany.Logo);
        createdCompany.OwnerId.Should().Be(userId);

        _userRepositoryMock.Verify(repo => repo.Update(It.Is<ApplicationUser>(u => u.Role == StringDefinitions.Owner)), Times.Once);
        _roleRepositoryMock.Verify(repo => repo.AddUserToRoleAsync(userId, StringDefinitions.Owner), Times.Once);
    }


    [Fact]
    public async Task GetPagedAndFilteredCompanies_WithSearchQuery_ReturnsFilteredCompanies()
    {
        // Arrange
        _context.Companies.Add(new Company { Id = 1, Name = "Company A", OwnerId = "owner1", Owner = new ApplicationUser { Name = "Owner A" } });
        _context.Companies.Add(new Company { Id = 2, Name = "Company B", OwnerId = "owner2", Owner = new ApplicationUser { Name = "Owner B" } });
        await _context.SaveChangesAsync();

        // Act
        var companies = await _companyRepository.GetAllCompaniesAsync();

        // Assert
        companies.First().Name.Should().Be("Company A");
    }

    [Fact]
    public async Task GetPagedAndFilteredCompanies_WithPagination_ReturnsPagedCompanies()
    {
        // Arrange
        var owner = new ApplicationUser { Id = "owner2", Name = "Owner Two" };
        var companies = Enumerable.Range(1, 20).Select(i =>
            new Company { Name = $"Company {i}", OwnerId = owner.Id }).ToList();

        await _context.Users.AddAsync(owner);
        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        // Act
        var companiesReturned = await _companyRepository.GetAllCompaniesAsync();

        // Assert
        companiesReturned.First().Name.Should().Be("Company 1");
    }

    [Fact]
    public async Task GetCompanyById_WithValidId_ReturnsCompany()
    {
        // Arrange
        var company = new Company { Name = "Valid Company", OwnerId = "owner3" };
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _companyRepository.GetByIdAsync(company.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(company.Name);
    }

    [Fact]
    public async Task GetAllByUserId_WithValidUserId_ReturnsAssociatedCompanies()
    {
        // Arrange
        var userId = "user123";

        var companies = new List<Company>
    {
        new Company { Id = 1, Name = "Company A", OwnerId = userId },
        new Company { Id = 2, Name = "Company B", OwnerId = userId }
    };

        // Ensure companies are added to the context first
        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        var rolesInCompany = companies.Select(c => new RoleInCompany
        {
            UserId = userId,
            CompanyId = c.Id,
            Name = StringDefinitions.Owner
        }).ToList();

        await _context.RoleInCompany.AddRangeAsync(rolesInCompany);
        await _context.SaveChangesAsync();

        // Act
        var associatedCompanyIds = await _companyRepository.GetAllByUserIdAsync(userId);

        // Assert
        associatedCompanyIds.Should().HaveCount(2);
        associatedCompanyIds.Should().BeEquivalentTo(companies.Select(c => c.Id));
    }
}
