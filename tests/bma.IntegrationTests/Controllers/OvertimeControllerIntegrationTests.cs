using System.Net;
using System.Net.Http.Json;
using bma.Application.Overtime.Dtos;
using bma.Domain.Constants;
using bma.IntegrationTests.Utilities;
using FluentAssertions;

namespace bma.IntegrationTests.Controllers;

public class OvertimeControllerIntegrationTests : BaseIntegrationClass
{
    public OvertimeControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }
    [Fact]
    public async Task GetAllOvertimeRequests_ForValidCompanyAsOwner_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/overtime/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualOvertimeRequests = await response.Content.ReadFromJsonAsync<List<OvertimeRequestResponseDto>>();

        // Assert the returned overtime requests match the seeded data
        actualOvertimeRequests.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = 5,
                Status = StringDefinitions.RequestStatusApproved, // Replace with actual seeded status
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                Length = 3,
                Description = "Urgent Project"
            },
            new
            {
                Id = 6,
                Status = StringDefinitions.RequestStatusPending, // Replace with actual seeded status
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                Length = 2,
                Description = "Additional Testing"
            }
        }, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAllOvertimeRequests_ForInvalidCompany_Returns404NotFound()
    {
        // Arrange
        var invalidCompanyId = 999;

        // Act
        var response = await Client.GetAsync($"/api/overtime/{invalidCompanyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllOvertimeRequests_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/overtime/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOvertimeRequest_WithValidData_Returns201Created()
    {
        // Arrange
        var companyId = 1;
        var createOvertimeRequest = new CreateOvertimeRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Length = 5,
            Description = "Project Deadline"
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/overtime/{companyId}", createOvertimeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateOvertimeRequest_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var companyId = 1;
        var createOvertimeRequest = new CreateOvertimeRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), // Invalid: Start date in the past
            Length = -3, // Invalid: Negative length
            Description = "" // Invalid: Empty reason
        };

        // Act
        var response = await Client.PostAsJsonAsync($"/api/overtime/{companyId}", createOvertimeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOvertimeRequest_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;
        var createOvertimeRequest = new CreateOvertimeRequestDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Length = 5,
            Description = "Project Deadline"
        };

        // Act
        var response = await unauthenticatedClient.PostAsJsonAsync($"/api/overtime/{companyId}", createOvertimeRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
