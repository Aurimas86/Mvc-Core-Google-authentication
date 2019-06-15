using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GoogleAuthentication.Services
{
    public interface ITokenService
    {
        Task<IDictionary<string, string>> RefreshAsync();
        void TryRevoke();
    }

    public class TokenService: ITokenService
    {
        private IUserService userService;

        public TokenService(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IDictionary<string, string>> RefreshAsync()
        {
            var url = "https://www.googleapis.com/oauth2/v4/token";
            using (var client = new HttpClient())
            {
                var request = new
                {
                    client_id = Startup.ClientId,
                    client_secret = Startup.ClientSecret,
                    refresh_token = userService.GetCurrent().RefreshToken,
                    grant_type = "refresh_token"
                };
                var response = await client.PostAsJsonAsync(url, request);
                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsAsync<Dictionary<string, string>>();
                    return tokenResponse;
                }
                else
                {
                    return null;
                }
            }
        }

        public void TryRevoke()
        {
            var refreshToken = userService.GetCurrent().RefreshToken;
            var url = $"https://accounts.google.com/o/oauth2/revoke?token={refreshToken}";
            using (var client = new HttpClient())
            {
                client.GetStringAsync(url);
            }
        }
    }
}
