using bma.Domain.Entities;
using bma.Domain.Exceptions;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace bma.UnitTests.RepositoryTests;

public class JoinRequestRepositoryTests : IDisposable
{
    private readonly BmaDbContext _context;
    private readonly JoinRequestRepository _repository;

    public JoinRequestRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BmaDbContext(options);
        _repository = new JoinRequestRepository(_context);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateJoinRequestAsync_WithValidData_CreatesJoinRequest()
    {
        // Arrange
        var userId = "user1";
        var companyId = 1;

        // Act
        await _repository.CreateJoinRequestAsync(userId, companyId);

        // Assert
        var joinRequest = await _context.JoinRequests.FirstOrDefaultAsync(jr => jr.UserId == userId && jr.CompanyId == companyId);
        joinRequest.Should().NotBeNull();
        joinRequest!.UserId.Should().Be(userId);
        joinRequest.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task GetJoinRequestForUserByCompanyAsync_WithValidData_ReturnsJoinRequest()
    {
        // Arrange
        var userId = "user1";
        var companyId = 1;
        var joinRequest = new JoinRequest
        {
            UserId = userId,
            CompanyId = companyId,
            AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        await _context.JoinRequests.AddAsync(joinRequest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetJoinRequestForUserByCompanyAsync(userId, companyId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.CompanyId.Should().Be(companyId);
        result.AcceptanceDate.Should().NotBeNull();
        result.AcceptanceDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task GetJoinRequestForUserByCompanyAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var companyId = 1;

        // Act
        var result = await _repository.GetJoinRequestForUserByCompanyAsync("invalidUser", companyId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetJoinRequestForUserByCompanyAsync_WithInvalidCompanyId_ReturnsNull()
    {
        // Arrange
        var userId = "user1";
        var joinRequest = new JoinRequest
        {
            UserId = userId,
            CompanyId = 1,
            AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        await _context.JoinRequests.AddAsync(joinRequest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetJoinRequestForUserByCompanyAsync(userId, 2);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateJoinRequestAsync_WithDuplicateData_DoesNotCreateNewJoinRequest()
    {
        // Arrange
        var userId = "user1";
        var companyId = 1;

        var joinRequest = new JoinRequest
        {
            UserId = userId,
            CompanyId = companyId
        };

        await _context.JoinRequests.AddAsync(joinRequest);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _repository.CreateJoinRequestAsync(userId, companyId);

        // Assert
        await act.Should().ThrowAsync<DuplicateJoinRequestException>()
            .WithMessage($"A join request already exists for UserId: {userId} and CompanyId: {companyId}.");
    }
    [Fact]
    public async Task GetAllJoinRequestsByCompanyIdAsync_WithValidCompanyId_ReturnsJoinRequests()
    {
        // Arrange
        var companyId = 1;
        var joinRequests = new List<JoinRequest>
    {
        new JoinRequest { UserId = "user1", CompanyId = companyId, AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow) },
        new JoinRequest { UserId = "user2", CompanyId = companyId, AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow) }
    };

        await _context.JoinRequests.AddRangeAsync(joinRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllJoinRequestsByCompanyIdAsync(companyId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(jr => jr.UserId == "user1");
        result.Should().Contain(jr => jr.UserId == "user2");
    }

    [Fact]
    public async Task GetAllJoinRequestsByCompanyIdAsync_WithNoJoinRequests_ReturnsEmptyList()
    {
        // Arrange
        var companyId = 1;

        // Act
        var result = await _repository.GetAllJoinRequestsByCompanyIdAsync(companyId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllJoinRequestsByCompanyIdAsync_WithInvalidCompanyId_ReturnsEmptyList()
    {
        // Arrange
        var companyId = 1;
        var joinRequest = new JoinRequest
        {
            UserId = "user1",
            CompanyId = companyId,
            AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await _context.JoinRequests.AddAsync(joinRequest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllJoinRequestsByCompanyIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllJoinRequestsByCompanyIdAsync_WithMultipleCompanies_ReturnsOnlySpecifiedCompanyJoinRequests()
    {
        // Arrange
        var companyId1 = 1;
        var companyId2 = 2;

        var joinRequests = new List<JoinRequest>
    {
        new JoinRequest { UserId = "user1", CompanyId = companyId1, AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow) },
        new JoinRequest { UserId = "user2", CompanyId = companyId1, AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow) },
        new JoinRequest { UserId = "user3", CompanyId = companyId2, AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow) }
    };

        await _context.JoinRequests.AddRangeAsync(joinRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllJoinRequestsByCompanyIdAsync(companyId1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(jr => jr.CompanyId == companyId1);
    }


}

