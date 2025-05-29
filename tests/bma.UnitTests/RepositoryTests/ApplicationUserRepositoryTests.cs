using bma.Domain.Entities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace bma.UnitTests.RepositoryTests;

public class ApplicationUserRepositoryTests : IDisposable
{
    private readonly DbContextOptions<BmaDbContext> _options;
    private readonly BmaDbContext _context;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public ApplicationUserRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new BmaDbContext(_options);

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidUserId_ReturnsUser()
    {
        // Arrange
        var repository = new ApplicationUserRepository(_context);
        var user = new ApplicationUser { Id = "1", Email = "testuser@example.com", Name = "Test User" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("1");
        result.Email.Should().Be("testuser@example.com");
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var repository = new ApplicationUserRepository(_context);

        // Act
        var result = await repository.GetByIdAsync("999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        var repository = new ApplicationUserRepository(_context);
        var user = new ApplicationUser { Id = "1", Email = "testuser@example.com", Name = "Test User" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("testuser@example.com");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("testuser@example.com");
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        var repository = new ApplicationUserRepository(_context);

        // Act
        var result = await repository.GetByEmailAsync("invalidemail@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_WithValidUser_UpdatesUser()
    {
        // Arrange
        var repository = new ApplicationUserRepository(_context);
        var user = new ApplicationUser { Id = "1", Email = "testuser@example.com", Name = "Old Name" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        user.Name = "Updated Name";
        repository.Update(user);
        await _context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync("1");
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
    }
}
