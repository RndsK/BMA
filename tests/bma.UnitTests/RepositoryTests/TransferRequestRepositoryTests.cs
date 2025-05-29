using bma.Domain.Entities.RequestEntities;
using bma.Domain.Entities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using bma.Domain.Constants;

namespace bma.UnitTests.RepositoryTests;

public class TransferRequestRepositoryTests : IDisposable
{
    private readonly BmaDbContext _context;
    private readonly FinancialRequestRepository _repository;

    public TransferRequestRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: "TransferRequestRepositoryTests")
            .Options;
        _context = new BmaDbContext(options);
        _repository = new FinancialRequestRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    [Fact]
    public async Task GetAllTransferRequestsByUserAsync_WithValidUserId_ReturnsUserTransferRequests()
    {
        // Arrange
        var userId = "user1";
        var participantEmail = "participant@example.com";

        var user = new ApplicationUser
        {
            Id = userId,
            Email = participantEmail
        };

        var signOffParticipantUser = new ApplicationUser
        {
            Id = "participant1",
            Email = "signoff@example.com"
        };

        var transferRequest = new FinancialRequest
        {
            Id = 1,
            UserId = userId,
            User = user,
            CompanyId = 1,
            Amount = 100,
            SignOffParticipants = new List<string>()
        };

        await _context.Users.AddRangeAsync(user, signOffParticipantUser);
        await _context.FinancialRequests.AddAsync(transferRequest);
        await _context.SaveChangesAsync();

        // Add SignOffParticipant
        var signOffParticipant = new TransferRequestSignOffParticipant
        {
            User = signOffParticipantUser,
            UserId = signOffParticipantUser.Id,
            RequestId = transferRequest.Id,
            Request = transferRequest
        };

        transferRequest.SignOffParticipants.Add(signOffParticipant.User.Email);
        await _context.TransferRequestSignOffParticipants.AddAsync(signOffParticipant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllFinancialRequestsForUserAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        var firstResult = result.First();
        firstResult.SignOffParticipants.Should().HaveCount(1);
        firstResult.SignOffParticipants.First().Should().Be(signOffParticipantUser.Email);
    }


    [Fact]
    public async Task GetAllTransferRequestsByCompanyAsync_WithValidCompanyId_ReturnsCompanyTransferRequests()
    {
        // Arrange
        var companyId = 1;

        var user = new ApplicationUser
        {
            Id = "user1",
            Email = "user@example.com"
        };

        var participant = new ApplicationUser
        {
            Id = "participant1",
            Email = "participant@example.com"
        };

        var transferRequest = new FinancialRequest
        {
            CompanyId = companyId,
            UserId = user.Id,
            Amount = 1000,
            SignOffParticipants = new List<string>()
        };

        await _context.Users.AddRangeAsync(user, participant);
        await _context.FinancialRequests.AddAsync(transferRequest);
        await _context.SaveChangesAsync();

        // Add SignOffParticipant
        var signOffParticipant = new TransferRequestSignOffParticipant
        {
            User = user,
            UserId = participant.Id,
            RequestId = transferRequest.Id, // Assign valid RequestId
            Status = StringDefinitions.SignOffStatusNotSigned
        };

        transferRequest.SignOffParticipants.Add(signOffParticipant.User.Email);
        await _context.TransferRequestSignOffParticipants.AddAsync(signOffParticipant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllFinancialRequestsForCompanyAsync(companyId);

        // Assert
        result.Should().HaveCount(1);
        var firstResult = result.First();
        firstResult.CompanyId.Should().Be(companyId);
        firstResult.SignOffParticipants.Should().HaveCount(1);
        firstResult.SignOffParticipants.First().Should().Be("user@example.com");
    }

    [Fact]
    public async Task GetAllTransferRequestsByUserAsync_WithNoMatchingUserId_ReturnsEmptyList()
    {
        // Arrange
        var userId = "nonexistent-user";
        var transferRequests = new List<FinancialRequest>
        {
            new FinancialRequest { Id = 1, UserId = "user1", CompanyId = 1, Amount = 100 }
        };

        await _context.FinancialRequests.AddRangeAsync(transferRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllFinancialRequestsForUserAsync(userId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllTransferRequestsByCompanyAsync_WithNoMatchingCompanyId_ReturnsEmptyList()
    {
        // Arrange
        var companyId = 999; // Non-existent company ID
        var transferRequests = new List<FinancialRequest>
        {
            new FinancialRequest { Id = 1, UserId = "user1", CompanyId = 1, Amount = 100 }
        };

        await _context.FinancialRequests.AddRangeAsync(transferRequests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllFinancialRequestsForCompanyAsync(companyId);

        // Assert
        result.Should().BeEmpty();
    }
}
