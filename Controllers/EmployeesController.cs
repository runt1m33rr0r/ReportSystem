using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReportSystem.Models;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IAuthService authService;

        public EmployeesController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthService authService)
        {
            this.userManager = userManager;
            this.authService = authService;
        }

        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel employee)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid employee!";

                return View();
            }

            const string roleName = "Employee";
            await this.authService.CreateRole(roleName);

            var user = new IdentityUser { UserName = employee.UserName, Email = employee.Email };
            var result = await this.userManager.CreateAsync(user, employee.Password);
            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(user, roleName);
                ViewData["Success"] = "Employee created!";

                return View();
            }
            else
            {
                ViewData["Error"] = "Could not create employee!";
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}