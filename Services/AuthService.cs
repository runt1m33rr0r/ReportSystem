using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ReportSystem.Services.Contracts;

namespace ReportSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AuthService(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
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
    }
}