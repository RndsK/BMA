using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace bma.UnitTests.RepositoryTests;

public class ExpensesRequestRepositoryTests : IDisposable
{
    private readonly BmaDbContext _context;
    private readonly ExpensesRequestRepository _repository;

    public ExpensesRequestRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: "ExpensesRequestRepositoryTests")
            .Options;
        _context = new BmaDbContext(options);
        _repository = new ExpensesRequestRepository(_context);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllExpensesForCompanyAsync_WithValidCompanyId_ReturnsCompanyExpenses()
    {
        // Arrange
        var companyId = 1;

        var user = new ApplicationUser
        {
            Id = "user1",
            UserName = "user1",
            Email = "test@test.com"
        };

        var expenses = new List<ExpensesRequest>
        {
            new ExpensesRequest { Id = 1, CompanyId = companyId, UserId = user.Id, Amount = 100, User = user },
            new ExpensesRequest { Id = 2, CompanyId = companyId, UserId = user.Id, Amount = 200, User = user  },
            new ExpensesRequest { Id = 3, CompanyId = 2, UserId = user.Id, Amount = 300, User = user  }
        };

        await _context.ExpensesRequests.AddRangeAsync(expenses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllExpenseRequestsForCompanyAsync(companyId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expenses.Where(e => e.CompanyId == companyId));
    }

    [Fact]
    public async Task GetAllExpensesForUserAsync_WithValidUserId_ReturnsUserExpenses()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user",
            UserName = "user1",
            Email = "test@test.com"
        };

        var user2 = new ApplicationUser
        {
            Id = "user2",
            UserName = "user2",
            Email = "test@test.com"
        };


        var expenses = new List<ExpensesRequest>
        {
            new ExpensesRequest { Id = 1, CompanyId = 1, UserId = user.Id, Amount = 100, User = user },
            new ExpensesRequest { Id = 2, CompanyId = 1, UserId = user2.Id, Amount = 200, User = user2 },
            new ExpensesRequest { Id = 3, CompanyId = 2, UserId = user.Id, Amount = 300, User = user }
        };

        await _context.ExpensesRequests.AddRangeAsync(expenses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllExpenseRequestsForUserAsync(user.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expenses.Where(e => e.UserId == user.Id));
    }

    [Fact]
    public async Task GetAllExpensesForCompanyAsync_WithNoMatchingCompanyId_ReturnsEmptyList()
    {
        // Arrange
        var companyId = 999; // Non-existent company ID
        var expenses = new List<ExpensesRequest>
        {
            new ExpensesRequest { Id = 1, CompanyId = 1, UserId = "user1", Amount = 100 }
        };

        await _context.ExpensesRequests.AddRangeAsync(expenses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllExpenseRequestsForCompanyAsync(companyId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllExpensesForUserAsync_WithNoMatchingUserId_ReturnsEmptyList()
    {
        // Arrange
        var userId = "nonexistent-user";
        var expenses = new List<ExpensesRequest>
        {
            new ExpensesRequest { Id = 1, CompanyId = 1, UserId = "user1", Amount = 100 }
        };

        await _context.ExpensesRequests.AddRangeAsync(expenses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllExpenseRequestsForUserAsync(userId);

        // Assert
        result.Should().BeEmpty();
    }
}
