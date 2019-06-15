using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoogleAuthentication.Controllers
{
    public class AccountController: Controller
    {
        private IAccountService accountService;
        private ITokenService tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            this.accountService = accountService;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(LoginCallback), new { returnUrl })
                });
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback(string returnUrl = "/")
        {
            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authResult.Succeeded)
            {
                // Google refuses to authenticate non https pages. Make sure launchSettings.json has sslPort set.
                return BadRequest();
            }

            await accountService.SignInNewAsync(authResult.Principal, authResult.Properties.Items);
            return LocalRedirect(returnUrl);
        }

        public Task Logout()
        {
            tokenService.TryRevoke();
            return accountService.SignOutAsync();
        }
    }
}
