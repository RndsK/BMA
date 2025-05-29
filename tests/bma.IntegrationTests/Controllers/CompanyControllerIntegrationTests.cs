using System.Net;
using System.Net.Http.Json;
using bma.Application.Companies.Dtos;
using bma.Application.JoinRequests.Dtos;
using bma.Domain.Constants;
using bma.IntegrationTests.Utilities;
using FluentAssertions;

namespace bma.IntegrationTests.Controllers;

public class CompanyControllerIntegrationTests : BaseIntegrationClass
{
    public CompanyControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }

    [Fact]
    public async Task CreateCompany_WithValidData_Returns201Created()
    {
        // Arrange
        var createCompanyDto = new CreateCompanyDto
        {
            Name = "Test Company",
            Country = "USA",
            Industry = "Technology",
            Description = "A tech company",
            Logo = "https://example.com/logo.png"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/company", createCompanyDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateCompany_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var createCompanyDto = new CreateCompanyDto
        {
            Name = "Unauthorised company",
            Country = "USA",
            Industry = "Technology",
            Description = "A tech company",
            Logo = "https://example.com/logo.png"
        };

        // Act
        var response = await unauthenticatedClient.PostAsJsonAsync("/api/company", createCompanyDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllCompanies_WithAuthentication_Returns200OK()
    {
        // Act
        var response = await Client.GetAsync("/api/company");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualCompanies = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();
        actualCompanies.Should().BeEquivalentTo(new[]
        {
            new { Id = 1, Name = "Test Company", OwnerName = "TestOwner" },
            new { Id = 2, Name = "Test Company2", OwnerName = "TestOwner" }
        });
    }

    [Fact]
    public async Task GetCompanyJoinRequests_WithValidCompany_Returns200OK()
    {
        // Arrange
        var companyId = 2;

        // Act
        var response = await Client.GetAsync($"/api/company/{companyId}/joinRequests");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualJoinRequests = await response.Content.ReadFromJsonAsync<List<JoinRequestDto>>();

        // Verify the returned join requests match seeded data
        actualJoinRequests.Should().NotBeNullOrEmpty();
        actualJoinRequests.Should().HaveCount(1);
        actualJoinRequests.First().Should().BeEquivalentTo(new
        {
            Id = 2,
            Status = StringDefinitions.RequestStatusPending,
            CompanyId = companyId,
            UserId = "TestUserId"
        });
    }

    [Fact]
    public async Task GetUserJoinRequests_WithValidUser_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/company/{companyId}/myJoinRequests");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualJoinRequests = await response.Content.ReadFromJsonAsync<List<JoinRequestDto>>();

        // Verify the returned join requests match seeded data
        actualJoinRequests.Should().NotBeNullOrEmpty();
        actualJoinRequests.Should().HaveCount(1);
        actualJoinRequests.First().Should().BeEquivalentTo(new
        {
            Id = 1,
            Status = StringDefinitions.RequestStatusApproved,
            CompanyId = companyId,
            UserId = "TestUserId"
        });
    }

    [Fact]
    public async Task RejectJoinRequest_WithValidData_Returns200OK()
    {
        // Arrange
        var joinRequestId = 2;

        // Act
        var response = await Client.PutAsync($"/api/company/{joinRequestId}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ApproveJoinRequest_WithValidData_Returns200OK()
    {
        // Arrange
        var joinRequestId = 2;

        // Act
        var response = await Client.PutAsync($"/api/company/{joinRequestId}/approve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task CreateJoinRequest_WithValidData_Returns201Created()
    {
        // Arrange
        var companyId = 1; // Ensure this company ID exists in the test database
        SwitchTestUser("TestUser2Id");

        // Act
        var response = await Client.PostAsync($"/api/company/join/{companyId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateJoinRequest_ForNonExistentCompany_Returns404NotFound()
    {
        // Arrange
        var nonExistentCompanyId = 999;

        // Act
        var response = await Client.PostAsync($"/api/company/join/{nonExistentCompanyId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateJoinRequest_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication(); // Disable authentication for this test
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.PostAsync($"/api/company/join/{companyId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCompany_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var createCompanyDto = new { Name = "" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/company", createCompanyDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}
