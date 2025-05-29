using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace bma.UnitTests.RepositoryTests;

public class OvertimeRequestRepositoryTests : IDisposable
{
    private readonly BmaDbContext _context;
    private readonly OvertimeRequestRepository _repository;

    public OvertimeRequestRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BmaDbContext(options);
        _repository = new OvertimeRequestRepository(_context);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Fact]
    public async Task GetAllOvertimeRequestsForCompanyAsync_WithValidCompanyId_ReturnsCompanyOvertimeRequests()
    {
        // Arrange
        var companyId = 1;
        var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = "user1", Email = "user1@example.com" },
        new ApplicationUser { Id = "user2", Email = "user2@example.com" },
        new ApplicationUser { Id = "user3", Email = "user3@example.com" }
    };

        var overtimeRequests = new List<OvertimeRequest>
    {
        new OvertimeRequest { Id = 1, CompanyId = companyId, UserId = "user1", User = users[0], StartDate = DateOnly.FromDateTime(DateTime.UtcNow), Length = 2, Description = "Project Deadline" },
        new OvertimeRequest { Id = 2, CompanyId = companyId, UserId = "user2", User = users[1], StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), Length = 1, Description = "Urgent Task" },
        new OvertimeRequest { Id = 3, CompanyId = 2, UserId = "user3", User = users[2], StartDate = DateOnly.FromDateTime(DateTime.UtcNow), Length = 3, Description = "Other Work" }
    };

        await _context.Users.AddRangeAsync(users);
        await _context.OvertimeRequests.AddRangeAsync(overtimeRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllOvertimeRequestsForCompanyAsync(companyId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(or => or.CompanyId == companyId);
        result.Select(or => or.Id).Should().Contain(new[] { 1, 2 });
    }

    [Fact]
    public async Task GetAllOvertimeRequestsForUserAsync_WithValidUserId_ReturnsUserOvertimeRequests()
    {
        // Arrange
        var userId = "user1";
        var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = userId, Email = "user1@example.com" },
        new ApplicationUser { Id = "user2", Email = "user2@example.com" }
    };

        var overtimeRequests = new List<OvertimeRequest>
    {
        new OvertimeRequest { Id = 1, UserId = userId, CompanyId = 1, User = users[0], StartDate = DateOnly.FromDateTime(DateTime.UtcNow), Length = 2, Description = "Project Deadline" },
        new OvertimeRequest { Id = 2, UserId = userId, CompanyId = 2, User = users[0], StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), Length = 1, Description = "Urgent Task" },
        new OvertimeRequest { Id = 3, UserId = "user2", CompanyId = 1, User = users[1], StartDate = DateOnly.FromDateTime(DateTime.UtcNow), Length = 1, Description = "Other Work" }
    };

        await _context.Users.AddRangeAsync(users);
        await _context.OvertimeRequests.AddRangeAsync(overtimeRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllOvertimeRequestsForUserAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(or => or.UserId == userId);
        result.Select(or => or.Id).Should().Contain(new[] { 1, 2 });
    }
}

