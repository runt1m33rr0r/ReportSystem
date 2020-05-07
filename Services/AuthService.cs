using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task CreateRole(string roleName)
        {
            if (await this.roleManager.FindByNameAsync(roleName) == null)
            {
                var role = new IdentityRole();
                role.Name = roleName;

                await this.roleManager.CreateAsync(role);
            }
        }

        public async Task<bool> UserWithRoleExists(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName) &&
                (await this.userManager.GetUsersInRoleAsync(roleName)).Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}