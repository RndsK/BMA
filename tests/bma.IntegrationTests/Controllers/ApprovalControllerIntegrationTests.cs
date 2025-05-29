using System.Net;
using System.Net.Http.Json;
using bma.Application.Approvals.Dtos;
using bma.Domain.Constants;
using bma.IntegrationTests.Utilities;
using FluentAssertions;

namespace bma.IntegrationTests.Controllers;

public class ApprovalControllerIntegrationTests : BaseIntegrationClass
{
    public ApprovalControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }

    [Fact]
    public async Task GetApprovalsByUserId_WithValidUser_Returns200OK()
    {
        // Act
        ResetDatabaseAndSeed();
        var response = await Client.GetAsync("/api/approvals/currentUser");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actualApprovals = await response.Content.ReadFromJsonAsync<List<ApprovalDto>>();
        actualApprovals.Should().BeEquivalentTo(new[]
        {
            new { Id = 1, Status = StringDefinitions.RequestStatusApproved, RequestId = 1, ApprovedBy = "testuser@example.com" },
            new { Id = 2, Status = StringDefinitions.RequestStatusRejected, RequestId = 2, ApprovedBy = "testuser@example.com" },
            new { Id = 3, Status = StringDefinitions.RequestStatusRejected, RequestId = 7, ApprovedBy = "testuser@example.com" }
        });
    }


    [Fact]
    public async Task GetApprovalsByUserId_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        ResetDatabaseAndSeed();
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();

        // Act
        var response = await unauthenticatedClient.GetAsync("/api/approvals/currentUser");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApprovalsByCompanyId_WithValidCompany_Returns200OK()
    {
        // Arrange
        ResetDatabaseAndSeed();
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/approvals/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var actualApprovals = await response.Content.ReadFromJsonAsync<List<ApprovalDto>>();
        actualApprovals.Should().BeEquivalentTo(new[]
        {
            new { Id = 1, Status = StringDefinitions.RequestStatusApproved, RequestId = 1, ApprovedBy = "testuser@example.com" },
            new { Id = 2, Status = StringDefinitions.RequestStatusRejected, RequestId = 2, ApprovedBy = "testuser@example.com" },
            new { Id = 3, Status = StringDefinitions.RequestStatusRejected, RequestId = 7, ApprovedBy = "testuser@example.com" }
        });

    }

    [Fact]
    public async Task GetApprovalsByCompanyId_WithInvalidCompany_Returns404NotFound()
    {
        // Arrange
        ResetDatabaseAndSeed();
        var invalidCompanyId = 999;

        // Act
        var response = await Client.GetAsync($"/api/approvals/company/{invalidCompanyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetApprovalsByCompanyId_AsUnauthorizedUser_Returns401Unauthorized()
    {
        // Arrange
        ResetDatabaseAndSeed();
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/approvals/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApprovalsByCompanyId_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        ResetDatabaseAndSeed();
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();

        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/approvals/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
