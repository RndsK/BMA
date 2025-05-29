using System.Net;
using System.Net.Http.Json;
using bma.IntegrationTests.Utilities;
using FluentAssertions;
using bma.Application.Expenses.Dtos;
using bma.Domain.Constants;

namespace bma.IntegrationTests.Controllers;

public class ExpensesControllerIntegrationTests : BaseIntegrationClass
{
    public ExpensesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }

    [Fact]
    public async Task GetAllExpenses_ForValidCompany_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/expenses/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);


        var actualExpenses = await response.Content.ReadFromJsonAsync<List<ExpenseRequestResponseDto>>();

        // Verify the returned expenses match the seeded data
        actualExpenses.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = 1,
                Status = StringDefinitions.RequestStatusPending, // Adjust based on the default seeded status
                Amount = 100m,
                Currency = "CHF",
                ExpenseType = "Office",
                ProjectName = "Project Alpha",
                Description = "Office supplies purchase",
                Attachment = "https://mockblobstorage.com/receipt1.png",
                Email = "testuser@example.com" // Use the email of the seeded user
            },
            new
            {
                Id = 2,
                Status = StringDefinitions.RequestStatusPending, // Adjust based on the default seeded status
                Amount = 200m,
                Currency = "CHF",
                ExpenseType = "Travel",
                ProjectName = "Project Beta",
                Description = "Travel expenses for client meeting",
                Attachment = "https://mockblobstorage.com/receipt2.png",
                Email = "testuser@example.com" // Use the email of the seeded user
            }
        }, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetAllExpenses_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/expenses/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateExpenseRequest_WithValidData_Returns201Created()
    {
        // Arrange
        var companyId = 1;
        var formData = new MultipartFormDataContent
            {
                { new StringContent("100"), "Amount" },
                { new StringContent("CHF"), "Currency" },
                { new StringContent("Travel"), "ExpenseType" },
                { new StringContent("Test Project"), "ProjectName" },
                { new StringContent("Test Comment"), "Description" }
            };

        var fileContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        formData.Add(fileContent, "receipt", "mock-receipt.png");

        // Act
        var response = await Client.PostAsync($"/api/expenses/{companyId}", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateExpenseRequest_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;
        var formData = new MultipartFormDataContent
    {
        { new StringContent("100"), "Amount" },
        { new StringContent("CHF"), "Currency" },
        { new StringContent("Travel"), "ExpenseType" },
        { new StringContent("Test Project"), "ProjectName" },
        { new StringContent("Test Comment"), "Description" }
    };

        // Act
        var response = await unauthenticatedClient.PostAsync($"/api/expenses/{companyId}", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateExpenseRequest_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var companyId = 1;
        var formData = new MultipartFormDataContent
            {
                { new StringContent("0"), "Amount" }, // Invalid Amount
                { new StringContent(""), "Currency" }, // Empty Currency
                { new StringContent(""), "ExpenseType" }, // Empty ExpenseType
                { new StringContent(""), "ProjectName" }, // Empty ProjectName
                { new StringContent(""), "Comments" } // Empty Comments
            };

        // Act
        var response = await Client.PostAsync($"/api/expenses/{companyId}", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    
}
