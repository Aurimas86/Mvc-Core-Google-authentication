using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoogleAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private IUserService userService;
        private IProjectService projectService;

        public HomeController(IUserService userService, IProjectService projectService)
        {
            this.userService = userService;
            this.projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            var user = userService.GetCurrent();
            ViewBag.User = user;
            var model = await projectService.GetProjects();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
