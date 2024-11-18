using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FitnessApp.Common.Tests.Fixtures;

public abstract class MockAuthenticationHandlerBase(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        List<Claim> claims = [..GetClaimsByRequest(Request.Path)];
        claims.Add(new Claim(ClaimTypes.NameIdentifier, MockConstants.SvTest));
        var identity = new ClaimsIdentity(claims, MockConstants.SvTest);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, MockConstants.Scheme);
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }

    protected virtual Claim[] GetClaimsByRequest(string path)
    {
        return [];
    }
}
