using bma.Domain.Entities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace bma.IntegrationTests.Utilities;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private bool _disableAuthentication;

    public void DisableAuthentication()
    {
        _disableAuthentication = true;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext configuration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<BmaDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<BmaDbContext>(options => { options.UseInMemoryDatabase("TestDb"); });

            // Mock BlobStorageService
            var blobStorageServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IBlobStorageService));
            if (blobStorageServiceDescriptor != null)
            {
                services.Remove(blobStorageServiceDescriptor);
            }

            var mockBlobStorageService = new Mock<IBlobStorageService>();
            mockBlobStorageService
                .Setup(x => x.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://mockblobstorage.com/fakefile");
            services.AddSingleton(mockBlobStorageService.Object);

            // Add authentication if not disabled
            if (!_disableAuthentication)
            {
                services.AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<BmaDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            }
        });

        builder.UseEnvironment("Testing");
    }
}
