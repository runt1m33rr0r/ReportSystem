using System.Threading.Tasks;

namespace ReportSystem.Services.Contracts
{
    public interface IAuthService
    {
        Task CreateRole(string roleName);
        Task<bool> UserWithRoleExists(string roleName);
    }
}