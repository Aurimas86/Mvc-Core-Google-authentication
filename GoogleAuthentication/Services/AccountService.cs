using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace GoogleAuthentication.Services
{
    public interface IAccountService
    {
        Task SignInNewAsync(ClaimsPrincipal principal, IDictionary<string, string> items);
        Task SignInRefreshAsync(ClaimsIdentity identity, IDictionary<string, string> items);
        Task SignOutAsync(string redirectUri = "/");
    }

    public class AccountService : IAccountService
    {
        private HttpContext httpContext;
        private static readonly TimeSpan AccessTokenExpiration = TimeSpan.FromMinutes(56);

        public AccountService(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        public Task SignInNewAsync(ClaimsPrincipal principal, IDictionary<string, string> items)
        {
            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim("email", principal.FindFirstValue(ClaimTypes.Email)),
                new Claim("name", principal.FindFirstValue(ClaimTypes.Name)),
                new Claim("picture", principal.FindFirstValue("urn:google:picture")),
                new Claim("access_token", items[".Token.access_token"]),
                new Claim("refresh_token", items[".Token.refresh_token"]),
                new Claim("token_type", items[".Token.token_type"]),
                new Claim("expires_at", DateTime.UtcNow.Add(AccessTokenExpiration).ToString())
            },
            Startup.ApplicationScheme);

            return httpContext.SignInAsync(Startup.ApplicationScheme, new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });
        }

        public Task SignInRefreshAsync(ClaimsIdentity claimsIdentity, IDictionary<string, string> items)
        {
            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst("access_token"));
            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst("token_type"));
            claimsIdentity.RemoveClaim(claimsIdentity.FindFirst("expires_at"));
            claimsIdentity.AddClaims(new[] {
                new Claim("access_token", items["access_token"]),
                new Claim("token_type", items["token_type"]),
                new Claim("expires_at", DateTime.UtcNow.Add(AccessTokenExpiration).ToString())
            });

            return httpContext.SignInAsync(Startup.ApplicationScheme, new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });
        }

        public Task SignOutAsync(string redirectUri = "/")
        {
            return httpContext.SignOutAsync(Startup.ApplicationScheme, new AuthenticationProperties
            {
                RedirectUri = redirectUri
            });
        }
    }
}
