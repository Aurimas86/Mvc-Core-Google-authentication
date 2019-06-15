using GoogleAuthentication.Filters;
using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleAuthentication
{
    public class Startup
    {
        public static string ApplicationScheme = "Application";
        // TODO: enter your Google ClientId & ClientSecret.
        public static string ClientId { get; } = "ClientId";
        public static string ClientSecret { get; } = "ClientSecret";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = ApplicationScheme;
            })
            .AddCookie(ApplicationScheme)
            .AddGoogle(options =>
            {
                options.ClientId = ClientId;
                options.ClientSecret = ClientSecret;
                options.SaveTokens = true;
                options.Scope.Add("https://www.googleapis.com/auth/cloud-platform");
                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                options.AccessType = "offline";
                options.AuthorizationEndpoint += "?prompt=consent";
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
                options.Filters.Add(new RefreshTokenFilter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IProjectService, ProjectService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
