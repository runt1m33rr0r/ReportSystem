using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportSystem.Models;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService authService;

        public HomeController(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            if (!(await this.authService.UserWithRoleExists("Admin")))
            {
                return RedirectToAction("CreateAdmin", "Admin");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
