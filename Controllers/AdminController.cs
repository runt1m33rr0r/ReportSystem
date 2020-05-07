using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReportSystem.Models;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IAuthService authService;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthService authService)
        {
            this.userManager = userManager;
            this.authService = authService;
        }

        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(AdminViewModel admin)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid admin!";

                return View();
            }

            const string roleName = "Admin";
            if (await this.authService.UserWithRoleExists(roleName))
            {
                ViewData["Error"] = "There is already an admin!";

                return View();
            }

            await this.authService.CreateRole(roleName);
            var user = new IdentityUser { UserName = "admin", Email = "admin@admin.admin" };
            var result = await this.userManager.CreateAsync(user, admin.Password);
            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(user, roleName);
                ViewData["Success"] = "Admin created!";

                return View();
            }
            else
            {
                ViewData["Error"] = "Could not create admin!";
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}