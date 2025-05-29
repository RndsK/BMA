using System.Net;
using System.Net.Http.Json;
using bma.Application.Holidays.Dtos;
using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.IntegrationTests.Utilities;
using FluentAssertions;

namespace bma.IntegrationTests.Controllers;

public class HolidayControllerIntegrationTests : BaseIntegrationClass
{
    public HolidayControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }

    [Fact]
    public async Task CreateHolidayRequest_WithValidData_Returns201Created()
    {
        // Arrange
        var companyId = 1;
        var createHolidayRequest = new CreateHolidayRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            Description = "Vacation"
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/holidays/request/{companyId}", createHolidayRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateHolidayRequest_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var companyId = 1;
        var createHolidayRequest = new CreateHolidayRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), // Invalid: StartDate > EndDate
            Description = ""
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/holidays/request/{companyId}", createHolidayRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateHolidayRequest_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();

        var companyId = 1;
        var createHolidayRequest = new CreateHolidayRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            Description = "Vacation"
        };

        // Act
        var response = await unauthenticatedClient.PostAsJsonAsync($"/api/holidays/request/{companyId}", createHolidayRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBankHolidaysByCountryCode_WithValidCountry_Returns200OK()
    {
        // Arrange
        var countryCode = "US";

        // Act
        var response = await Client.GetAsync($"/api/holidays/bank-holidays/{countryCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualHolidays = await response.Content.ReadFromJsonAsync<List<DateTime>>();
        actualHolidays.Should().NotBeNullOrEmpty();

        actualHolidays.Should().Contain(DateTime.Parse($"{DateTime.UtcNow.Year}-01-01")); // Replace with a known holiday
    }

    [Fact]
    public async Task GetBankHolidaysByCountryCode_WithInvalidCountry_Returns404NotFound()
    {
        // Arrange
        var countryCode = "INVALID";

        // Act
        var response = await Client.GetAsync($"/api/holidays/bank-holidays/{countryCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetHolidayBalanceForUser_WithValidData_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/holidays/balance/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualBalance = await response.Content.ReadFromJsonAsync<HolidayBalance>();
        actualBalance.Should().NotBeNull();

        // Assert the values in the HolidayBalance object
        actualBalance!.Balance.Should().Be(-7); // Adjust as needed
        actualBalance.UpcomingHolidays.Should().Be(12); // Adjust as needed
        actualBalance.BalanceAfter.Should().Be(-19); // Adjust as needed
    }


    [Fact]
    public async Task GetHolidayBalanceForUser_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();

        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/holidays/balance/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUpcomingHolidaysForUser_WithValidData_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/holidays/upcoming/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualHolidays = await response.Content.ReadFromJsonAsync<List<HolidayRequestResponseDto>>();
        actualHolidays.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = 3,
                Status = StringDefinitions.RequestStatusApproved,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                Description = "Vacation"
            }
        });
    }


    [Fact]
    public async Task GetCompanyHolidays_WithValidData_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/holidays/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualHolidays = await response.Content.ReadFromJsonAsync<List<HolidayRequestResponseDto>>();
        actualHolidays.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = 4,
                Status = StringDefinitions.RequestStatusPending,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)),
                Description = "Vacation"
            },
            new
            {
                Id = 3,
                Status = StringDefinitions.RequestStatusApproved,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                Description = "Vacation"
            }
        });
    }

    [Fact]
    public async Task GetCompanyHolidays_WithInvalidMonth_Returns400BadRequest()
    {
        // Arrange
        var companyId = 1;
        var searchMonth = 13; // Invalid month

        // Act
        var response = await Client.GetAsync($"/api/holidays/company/{companyId}?searchMonth={searchMonth}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
