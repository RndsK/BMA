using System.Net;
using bma.IntegrationTests.Utilities;
using FluentAssertions;

namespace bma.IntegrationTests.Controllers;
public class RequestControllerIntegrationTests : BaseIntegrationClass
{
    public RequestControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }
    [Fact]
    public async Task RejectRequest_WithValidRequest_Returns200OK()
    {
        // Arrange
        var requestId = 1; // Ensure this request ID exists in the seeded data

        // Act
        var response = await Client.PutAsync($"/api/requests/{requestId}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The request has been successfully rejected.");
    }

    [Fact]
    public async Task RejectRequest_WithInvalidRequestId_Returns404NotFound()
    {
        // Arrange
        var invalidRequestId = 999; // Ensure this request ID does not exist

        // Act
        var response = await Client.PutAsync($"/api/requests/{invalidRequestId}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RejectRequest_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var requestId = 1;

        // Act
        var response = await unauthenticatedClient.PutAsync($"/api/requests/{requestId}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ApproveRequest_WithValidRequest_Returns200OK()
    {
        // Arrange
        var requestId = 1; // Ensure this request ID exists in the seeded data

        // Act
        var response = await Client.PutAsync($"/api/requests/{requestId}/approve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("The request has been successfully approved.");
    }

    [Fact]
    public async Task ApproveRequest_WithInvalidRequestId_Returns404NotFound()
    {
        // Arrange
        var invalidRequestId = 999; // Ensure this request ID does not exist

        // Act
        var response = await Client.PutAsync($"/api/requests/{invalidRequestId}/approve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ApproveRequest_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var requestId = 1;

        // Act
        var response = await unauthenticatedClient.PutAsync($"/api/requests/{requestId}/approve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPendingCompanyRequests_WithValidData_Returns200OK()
    {
        // Arrange
        var companyId = 1; // Ensure this company ID exists in the seeded data
        var pageNumber = 1;
        var pageSize = 10;

        // Act
        var response = await Client.GetAsync($"/api/requests/company/{companyId}?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPendingCompanyRequests_WithInvalidCompanyId_Returns404NotFound()
    {
        // Arrange
        var invalidCompanyId = 999; // Ensure this company ID does not exist

        // Act
        var response = await Client.GetAsync($"/api/requests/company/{invalidCompanyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPendingCompanyRequests_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/requests/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
