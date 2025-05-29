using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using bma.UnitTests.RepositoryTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace bma.UnitTests.RepositoryTests;

public class HolidayRequestRepositoryTests :IDisposable
{
    private readonly DbContextOptions<BmaDbContext> _options;
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly BmaDbContext _context;

    public HolidayRequestRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: "HolidayRequestTestDb")
            .Options;

        _context = new BmaDbContext(_options);
        _httpClientMock = new Mock<HttpClient>();
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetBankHolidayDatesAsync_WithValidCountryCode_ReturnsHolidays()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("[{\"date\": \"2024-01-01\"}, {\"date\": \"2024-12-25\"}]")
        };

        var mockHttpHandler = new MockHttpMessageHandler(_ => mockResponse);
        var httpClient = new HttpClient(mockHttpHandler);

        var repository = new HolidayRequestRepository(_context, httpClient);

        // Act
        var holidays = await repository.GetBankHolidayDatesAsync("US");

        // Assert
        holidays.Should().Contain(new[] { DateTime.Parse("2024-01-01"), DateTime.Parse("2024-12-25") });
    }

    [Fact]
    public async Task GetApprovedUpcomingHolidaysByUserIdAsync_WithValidUserId_ReturnsHolidays()
    {
        // Arrange
        var userId = "test-user-id";
        var companyId = 1;

        var user = new ApplicationUser
        {
            Id = userId,
            Name = "Test User",
            Email = "testuser@example.com"
        };

        var company = new Company
        {
            Id = companyId,
            Name = "Test Company",
            OwnerId = userId
        };

        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

        var holidayRequest = new HolidayRequest
        {
            Id = 1,
            UserId = userId,
            CompanyId = companyId,
            StartDate = futureDate,
            EndDate = futureDate.AddDays(5),
            Status = StringDefinitions.RequestStatusApproved,
            User = user,
            Company = company
        };

        await _context.Users.AddAsync(user);
        await _context.Companies.AddAsync(company);
        await _context.HolidayRequests.AddAsync(holidayRequest);
        await _context.SaveChangesAsync();

        var repository = new HolidayRequestRepository(_context, _httpClientMock.Object);

        // Act
        var holidays = await repository.GetApprovedUpcomingHolidaysByUserIdAsync(userId, companyId);

        // Assert
        holidays.Should().HaveCount(1);
        holidays.First().Should().BeEquivalentTo(holidayRequest);
    }

    [Fact]
    public async Task GetHolidaysByCompanyId_WithValidCompanyId_ReturnsCompanyHolidays()
    {
        // Arrange
        var companyId = 1;

        var user = new ApplicationUser
        {
            Id = "user1",
            Name = "Test User",
            Email = "testuser@example.com"
        };

        var company = new Company
        {
            Id = companyId,
            Name = "Test Company",
            OwnerId = "user1"
        };

        var holiday1 = new HolidayRequest
        {
            Id = 1,
            UserId = user.Id,
            CompanyId = companyId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)),
            Status = StringDefinitions.RequestStatusApproved,
            User = user,
            Company = company
        };

        var holiday2 = new HolidayRequest
        {
            Id = 2,
            UserId = user.Id,
            CompanyId = companyId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1).AddDays(5)),
            Status = StringDefinitions.RequestStatusApproved,
            User = user,
            Company = company
        };

        await _context.Users.AddAsync(user);
        await _context.Companies.AddAsync(company);
        await _context.HolidayRequests.AddRangeAsync(holiday1, holiday2);
        await _context.SaveChangesAsync();

        var repository = new HolidayRequestRepository(_context, _httpClientMock.Object);

        // Act
        var holidays = await repository.GetHolidaysByCompanyAsync(companyId, null);

        // Assert
        holidays.Should().HaveCount(2);
        holidays.Should().ContainEquivalentOf(holiday1);
        holidays.Should().ContainEquivalentOf(holiday2);
    }

    [Fact]
    public async Task CreateHolidayRequestAsync_WithValidData_CreatesHolidayRequest()
    {
        // Arrange
        var repository = new HolidayRequestRepository(_context, null);
        var holidayRequest = new HolidayRequest
        {
            Id = 1,
            UserId = "user1",
            CompanyId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Status = StringDefinitions.RequestStatusPending
        };

        // Act
        await repository.CreateHolidayRequestAsync("user1", 1, holidayRequest);

        // Assert
        var result = await _context.HolidayRequests.FirstOrDefaultAsync(hr => hr.Id == 1);
        result.Should().NotBeNull();
        result!.UserId.Should().Be("user1");
        result.CompanyId.Should().Be(1);
    }

    [Fact]
    public async Task GetHolidaysByUserIdAsync_WithValidUserId_ReturnsHolidays()
    {
        // Arrange
        var repository = new HolidayRequestRepository(_context, null); // No mapper required here.
        var userId = "user1";
        var companyId = 1;

        // Seed data
        _context.Users.Add(new ApplicationUser { Id = userId, Email = "user1@example.com", UserName = "User1" });
        _context.HolidayRequests.AddRange(
            new HolidayRequest { Id = 1, UserId = userId, CompanyId = companyId, Status = StringDefinitions.RequestStatusApproved },
            new HolidayRequest { Id = 2, UserId = userId, CompanyId = companyId, Status = StringDefinitions.RequestStatusPending },
            new HolidayRequest { Id = 3, UserId = "user2", CompanyId = companyId, Status = StringDefinitions.RequestStatusApproved }
        );
        await _context.SaveChangesAsync();

        // Act
        var holidays = await repository.GetHolidaysByUserAsync(userId, companyId);

        // Assert
        holidays.Should().NotBeNull(); // Ensure the result is not null
        holidays.Should().HaveCount(2); // Only user1's holidays in company 1
        holidays.Select(h => h.Id).Should().Contain(new[] { 1, 2 }); // Validate the IDs of returned holidays
    }

}
