using GoogleAuthentication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace GoogleAuthentication.Services
{
    public interface IUserService
    {
        UserModel GetCurrent(bool fresh = false);
    }

    public class UserService : IUserService
    {
        private HttpContext httpContext;
        private UserModel current;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        public UserModel GetCurrent(bool fresh = false)
        {
            if (current != null && !fresh)
            {
                return current;
            }
            var user = httpContext.User;
            current = new UserModel
            {
                Email = user.FindFirstValue("email"),
                Name = user.FindFirstValue("name"),
                Picture = user.FindFirstValue("picture"),
                AccessToken = user.FindFirstValue("access_token"),
                TokenType = user.FindFirstValue("token_type"),
                RefreshToken = user.FindFirstValue("refresh_token"),
                ExpiresAt = DateTime.Parse(user.FindFirstValue("expires_at"))
            };
            return current;
        }
    }
}
