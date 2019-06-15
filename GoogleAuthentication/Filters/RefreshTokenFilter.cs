using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleAuthentication.Filters
{
    public class RefreshTokenFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if ((context.ActionDescriptor as ControllerActionDescriptor).ControllerName == "Account")
            {
                return;
            }
            var userService = context.HttpContext.RequestServices.GetService<IUserService>();
            var user = userService.GetCurrent();
            if (user.ExpiresAt > DateTime.UtcNow)
            {
                return;
            }

            var tokenService = context.HttpContext.RequestServices.GetService<ITokenService>();
            var tokenMap = await tokenService.RefreshAsync();
            if (tokenMap != null)
            {
                var accountService = context.HttpContext.RequestServices.GetService<IAccountService>();
                var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
                await accountService.SignInRefreshAsync(claimsIdentity, tokenMap);
                userService.GetCurrent(fresh: true);
            }
            else
            {
                context.Result = new RedirectToActionResult("Logout", "Account", new { });
            }
        }
    }
}
