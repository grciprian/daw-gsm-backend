using GSM.Dtos;
using GSM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDto registerDto)
        {
            if (registerDto.Password.Equals(registerDto.ConfirmPassword))
            {
                var user = await _accountService.Register(registerDto);

                if (user == null)
                    return new StatusCodeResult(500);

                return Ok(user);
            }

            return BadRequest("Confirm Password doesn't match with Password.");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _accountService.Login(loginDto);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect." });
            }

            return Ok(user);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        [Route("getAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var users = await _accountService.GetAllEmployees();

            if (users == null)
            {
                return Ok(new List<ApplicationUserDto>());
            }

            return Ok(users);
        }


        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [Route("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto.Password.Equals(employeeDto.ConfirmPassword))
            {
                var user = await _accountService.CreateEmployee(employeeDto);

                if (user == null)
                    return new StatusCodeResult(500);

                return Ok(user);
            }

            return BadRequest("Confirm Password doesn't match with Password.");
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [Route("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var result = await _accountService.DeleteEmployee(id);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}", Name = "GetSingleUserById")]
        public async Task<IActionResult> GetSingleUserById(string id)
        {
            var user = await _accountService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _accountService.GetCurrentUserAsync();
            var isCurrentUserCustomer = await _accountService.IsCurrentUserInRole(Role.Customer);

            if (isCurrentUserCustomer && !currentUser.Id.Equals(user.Id))
            {
                return Forbid();
            }

            return Ok(user);
        }

        [Authorize(Roles = Role.Employee)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _accountService.GetAll();

            if (users == null)
            {
                return Ok(new List<ApplicationUserDto>());
            }

            return Ok(users);
        }

        //[HttpPut]
        //[Route("{id}")]
        //public IActionResult UpdateCustomer(Guid id, [FromBody] ApplicationUserDto customerDto)
        //{
        //    ApplicationUser existingCustomer = _customerRepository.GetSingleById(id);
        //    if (existingCustomer == null)
        //    {
        //        return NotFound();
        //    }
        //    _mapper.Map(customerDto, existingCustomer);
        //    _customerRepository.Update(existingCustomer);
        //    bool result = _customerRepository.Save();
        //    if (!result)
        //    {
        //        return new StatusCodeResult(500);
        //    }
        //    return Ok(_mapper.Map<ApplicationUserDto>(existingCustomer));
        //}
    }
}
