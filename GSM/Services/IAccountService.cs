using GSM.Dtos;
using GSM.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GSM.Services
{
    public interface IAccountService
    {
        Task<ApplicationUser> GetChosenEmployee();
        Task<ApplicationUserDto> CreateEmployee(EmployeeDto employeeDto);
        Task<bool> DeleteEmployee(string id);
        Task<ApplicationUserDto> Register(RegisterDto registerDto);
        Task<ApplicationUserDto> Login(LoginDto loginDto);
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<bool> IsCurrentUserInRole(string role);
        Task<ApplicationUserDto> GetById(string id);
        IQueryable<ApplicationUserDto> GetAll();
        Task<IQueryable<ApplicationUserDto>> GetAllEmployees();
        Task<IQueryable<ApplicationUserDto>> GetAllCustomers();
    }
}
