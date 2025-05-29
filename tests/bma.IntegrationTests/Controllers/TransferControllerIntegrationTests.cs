using System.Net;
using System.Net.Http.Json;
using bma.Application.Transfer.Finacial.Dtos;
using bma.Application.Transfer.SignOff.Dtos;
using bma.Domain.Constants;
using bma.IntegrationTests.Utilities;
using FluentAssertions;

namespace bma.IntegrationTests.Controllers;

public class TransferControllerIntegrationTests : BaseIntegrationClass
{
    public TransferControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        ResetDatabaseAndSeed();
    }

    // Financial Requests Tests
    [Fact]
    public async Task GetAllFinancialRequests_AsOwner_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/transfer/{companyId}/financial");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var financialRequests = await response.Content.ReadFromJsonAsync<List<FinancialRequestResponseDto>>();
        financialRequests.Should().NotBeNull();
        financialRequests.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = 7,
                Type = "Budget",
                RecurrenceType = "One-Off",
                Currency = "CHF",
                Amount = 1000m,
                TransferFrom = "Account A",
                TransferTo = "Account B",
                Description = "Project Funding",
                Status = StringDefinitions.RequestStatusPending
            },
            new
            {
                Id = 8,
                Type = "Investment",
                RecurrenceType = "Monthly",
                Currency = "USD",
                Amount = 5000m,
                TransferFrom = "Account X",
                TransferTo = "Account Y",
                Description = "Long-term Investment",
                Status = StringDefinitions.RequestStatusApproved
            }
        });
    }

    [Fact]
    public async Task CreateFinancialRequest_WithValidData_Returns201Created()
    {
        // Arrange
        var companyId = 1;
        var createFinancialRequestDto = new CreateFinancialRequestDto
        {
            Type = "Budget",
            RecurrenceType = "One-Off",
            Currency = "CHF",
            Amount = 1500,
            TransferFrom = "Account C",
            TransferTo = "Account D",
            Description = "New Budget Allocation",
            SignOffParticipants = new List<string> { "testuser@example.com", "testuser2@example.com" }
        };

        var formData = new MultipartFormDataContent
        {
            { new StringContent(createFinancialRequestDto.Type), "Type" },
            { new StringContent(createFinancialRequestDto.RecurrenceType), "RecurrenceType" },
            { new StringContent(createFinancialRequestDto.Currency), "Currency" },
            { new StringContent(createFinancialRequestDto.Amount.ToString()), "Amount" },
            { new StringContent(createFinancialRequestDto.TransferFrom), "TransferFrom" },
            { new StringContent(createFinancialRequestDto.TransferTo), "TransferTo" },
            { new StringContent(createFinancialRequestDto.Description), "Description" },
        };

        foreach (var email in createFinancialRequestDto.SignOffParticipants)
        {
            formData.Add(new StringContent(email), "SignOffParticipants");
        }

        // Act
        var response = await Client.PostAsync($"/api/transfer/{companyId}/financial", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    [Fact]
    public async Task GetAllSignOffRequests_AsOwner_Returns200OK()
    {
        // Arrange
        var companyId = 1;

        // Act
        var response = await Client.GetAsync($"/api/transfer/{companyId}/signOff");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var signOffRequests = await response.Content.ReadFromJsonAsync<List<SignOffRequestResponseDto>>();
        signOffRequests.Should().NotBeNull();
        signOffRequests.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = 9,
                Description = "Approval for project"
            }
        });
    }

    [Fact]
    public async Task GetAllSignOffRequests_AsUnauthorizedUser_Returns401Unauthorized()
    {
        // Arrange
        var factory = new CustomWebApplicationFactory<Program>();
        factory.DisableAuthentication();
        using var unauthenticatedClient = factory.CreateClient();
        var companyId = 1;

        // Act
        var response = await unauthenticatedClient.GetAsync($"/api/transfer/{companyId}/signOff");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task CreateSignOffRequest_WithInvalidData_Returns400BadRequest()
    {
        // Arrange
        var companyId = 1;
        var createSignOffRequestDto = new CreateSignOffRequestDto
        {
            Description = "" // Invalid: Empty description
        };

        var formData = new MultipartFormDataContent
    {
        { new StringContent(createSignOffRequestDto.Description), "Description" },
        { new StringContent("test@example.com"), "SignOffParticipants" } // Add a participant for validity
    };

        // Set the correct content type
        formData.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

        // Act
        var response = await Client.PostAsync($"/api/transfer/{companyId}/signOff", formData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}
