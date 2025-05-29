using bma.Domain.Entities;
using bma.Infrastructure.Data;
using bma.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace bma.UnitTests.RepositoryTests;

public class ApplicationUserRoleRepositoryTests : IDisposable
{
    private readonly DbContextOptions<BmaDbContext> _options;
    private readonly BmaDbContext _context;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public ApplicationUserRoleRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<BmaDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new BmaDbContext(_options);

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null
        );
    }
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task IsUserInRole_WithUserInRole_ReturnsTrue()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "testuser@example.com" };
        _userManagerMock.Setup(m => m.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin"))
            .ReturnsAsync(true);

        var repository = new ApplicationUserRoleRepository(_context, _userManagerMock.Object);

        // Act
        var result = await repository.IsUserInRole("1", "Admin");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUserInRole_WithUserNotInRole_ReturnsFalse()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "testuser@example.com" };
        _userManagerMock.Setup(m => m.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin"))
            .ReturnsAsync(false);

        var repository = new ApplicationUserRoleRepository(_context, _userManagerMock.Object);

        // Act
        var result = await repository.IsUserInRole("1", "Admin");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsUserInRole_WithInvalidUserId_ReturnsFalse()
    {
        // Arrange
        _userManagerMock.Setup(m => m.FindByIdAsync("999"))
            .ReturnsAsync((ApplicationUser)null);

        var repository = new ApplicationUserRoleRepository(_context, _userManagerMock.Object);

        // Act
        var result = await repository.IsUserInRole("999", "Admin");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddUserToRoleAsync_WithValidUser_AddsRole()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "testuser@example.com" };
        _userManagerMock.Setup(m => m.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock.Setup(m => m.AddToRoleAsync(user, "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        var repository = new ApplicationUserRoleRepository(_context, _userManagerMock.Object);

        // Act
        await repository.AddUserToRoleAsync("1", "Admin");

        // Assert
        _userManagerMock.Verify(m => m.AddToRoleAsync(user, "Admin"), Times.Once);
    }

    [Fact]
    public async Task AddUserToRoleAsync_WithInvalidUser_DoesNotAddRole()
    {
        // Arrange
        _userManagerMock.Setup(m => m.FindByIdAsync("999"))
            .ReturnsAsync((ApplicationUser)null);

        var repository = new ApplicationUserRoleRepository(_context, _userManagerMock.Object);

        // Act
        await repository.AddUserToRoleAsync("999", "Admin");

        // Assert
        _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Admin"), Times.Never);
    }
}
