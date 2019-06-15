using Google.Apis.CloudResourceManager.v1.Data;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GoogleAuthentication.Services
{
    public interface IProjectService
    {
        Task<IList<Project>> GetProjects();
    }

    public class ProjectService: IProjectService
    {
        private IUserService userService;

        public ProjectService(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IList<Project>> GetProjects()
        {
            var user = userService.GetCurrent();
            var url = "https://cloudresourcemanager.googleapis.com/v1/projects";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.TokenType, user.AccessToken);
                var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsAsync<ListProjectsResponse>();
                return result.Projects;
            }
        }
    }
}
