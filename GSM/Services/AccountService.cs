using AutoMapper;
using GSM.Dtos;
using GSM.Helpers;
using GSM.Models;
using GSM.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GSM.Services
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IContractRepository _contractRepository;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            IContractRepository contractRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;

            _contractRepository = contractRepository;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> IsCurrentUserInRole(string role)
        {
            var currentUser = await GetCurrentUserAsync();
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            if (currentUserRoles.Contains(role))
            {
                return true;
            }
            return false;
        }

        public async Task<ApplicationUserDto> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return _mapper.Map<ApplicationUserDto>(user);
        }

        public IQueryable<ApplicationUserDto> GetAll()
        {
            var users = _userManager.Users;
            return _mapper.ProjectTo<ApplicationUserDto>(users);
        }

        public async Task<ApplicationUser> GetChosenEmployee()
        {
            var employees = await _userManager.GetUsersInRoleAsync(Role.Employee);
            var contracts = _contractRepository.GetAll();

            var query = employees
                .Join(
                contracts,
                e => e.Id,
                c => c.Id,
                (e, c) => new { Employee = e, ContractId = c.Id })
                .GroupBy(
                jo => jo.Employee,
                jo => jo.ContractId,
                (key, g) => new { Employee = key, ContractsCount = g.ToList().Count }
                ).ToList();

            var minValue = int.MaxValue;
            ApplicationUser chosenEmployee = null;
            for(int i = 0; i < query.Count; i++)
            {
                if(query[i].ContractsCount < minValue)
                {
                    minValue = query[i].ContractsCount;
                    chosenEmployee = query[i].Employee;
                }
            }

            if(chosenEmployee == null && employees.Count != 0)
            {
                var random = new Random();
                int index = random.Next(employees.Count);
                chosenEmployee = employees[index];
            }

            return chosenEmployee;
        }

        public async Task<IQueryable<ApplicationUserDto>> GetAllEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync(Role.Employee);
            return _mapper.ProjectTo<ApplicationUserDto>(employees.AsQueryable());
        }

        public async Task<IQueryable<ApplicationUserDto>> GetAllCustomers()
        {
            var customers = await _userManager.GetUsersInRoleAsync(Role.Customer);
            return _mapper.ProjectTo<ApplicationUserDto>(customers.AsQueryable());
        }

        public async Task<ApplicationUserDto> CreateEmployee(EmployeeDto employeeDto)
        {
            var user = new ApplicationUser { UserName = employeeDto.Email, Email = employeeDto.Email, PhoneNumber = employeeDto.PhoneNumber };
            var result = await _userManager.CreateAsync(user, employeeDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role.Employee);
                return _mapper.Map<ApplicationUserDto>(user);
            }

            return null;
        }

        public async Task<bool> DeleteEmployee(string id)
        {
            var user = _userManager.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<ApplicationUserDto> Register(RegisterDto registerDto)
        {
            var user = new ApplicationUser { UserName = registerDto.Email, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role.Customer);
                return _mapper.Map<ApplicationUserDto>(user);
            }

            return null;
        }

        public async Task<ApplicationUserDto> Login(LoginDto loginDto)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
            if (!signInResult.Succeeded)
                return null;

            var user = await _userManager.FindByNameAsync(loginDto.Email);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            ClaimsIdentity identity = GetClaimsIdentity(user, roles);

            var mappedUser = _mapper.Map<ApplicationUserDto>(user);
            mappedUser.Token = GetJwtToken(identity);
            mappedUser.Roles = roles;
            return mappedUser;
        }

        private ClaimsIdentity GetClaimsIdentity(ApplicationUser user, IList<string> roles)
        {
            // Here we can save some values to token.
            // For example we are storing here user id and email
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

            // Adding roles code
            // Roles property is string collection but you can modify Select code if it it's not
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claimsIdentity;
        }

        private string GetJwtToken(ClaimsIdentity identity)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                notBefore: DateTime.UtcNow,
                claims: identity.Claims,
                // our token will live 1 hour, but you can change you token lifetime here
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(24)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

    }
}
