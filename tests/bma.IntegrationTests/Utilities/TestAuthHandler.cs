using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace bma.IntegrationTests.Utilities;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Determine which user to authenticate based on a custom header
        var userId = Context.Request.Headers["X-Test-User"].ToString();

        var claims = userId switch
        {
            "TestUserId" => new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "TestUserId"),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
                new Claim(ClaimTypes.Role, "Owner")
            },
            "TestUser2Id" => new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "TestUser2Id"),
                new Claim(ClaimTypes.Email, "testuser2@example.com"),
                new Claim(ClaimTypes.Role, "User")
            },
            _ => null
        };

        if (claims == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid user ID"));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

}
