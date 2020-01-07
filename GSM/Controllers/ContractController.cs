using AutoMapper;
using GSM.Dtos;
using GSM.Models;
using GSM.Repositories;
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
    public class ContractController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly IGadgetRepository _gadgetRepository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public ContractController(IContractRepository contractRepository, IGadgetRepository gadgetRepository, IMapper mapper, IAccountService accountService)
        {
            _contractRepository = contractRepository;
            _gadgetRepository = gadgetRepository;
            _mapper = mapper;
            _accountService = accountService;
        }

        [Authorize(Roles = Role.Employee)]
        [HttpGet]
        [Route("getById/{id}", Name = "GetSingleContractById")]
        public IActionResult GetSingleContractById(string id)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ContractDto>(contract));
        }

        [Authorize(Roles = Role.Employee)]
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var contracts = _contractRepository.GetAll();
            if (contracts == null)
            {
                return Ok(new List<ContractDto>());
            }
            return Ok(contracts);
        }

        [Authorize(Roles = Role.Employee)]
        [HttpGet]
        [Route("getAllForCurrentEmployee")]
        public async Task<IActionResult> GetAllForCurrentEmployee()
        {
            var currentUser = await _accountService.GetCurrentUserAsync();

            var contracts = _contractRepository.GetAllByEmployeeId(currentUser.Id);
            var gadgets = _gadgetRepository.GetAll();
            var customers = await _accountService.GetAllCustomers();

            var query = contracts
                .Join(
                    gadgets,
                    c => c.GadgetId,
                    g => g.Id,
                    (c, g) => new {
                        c.Id,
                        CustomerName = g.Customer.UserName,
                        GadgetName = g.Name,
                        c.StartDate,
                        c.EndDate,
                        c.Status,
                        c.Observations
                    });


            if (contracts == null)
            {
                return Ok(new List<ContractDto>());
            }
            return Ok(query);
        }

        [HttpGet]
        [Route("getAllByGadgetId/{gadgetId}")]
        public IActionResult GetAllByGadgetId(string gadgetId)
        {
            IQueryable<Contract> contracts = _contractRepository.GetAllByGadgetId(gadgetId);
            if (contracts == null || contracts.Count() == 0)
            {
                return Ok(new List<ApplicationUserDto>());
            }
            return Ok(_mapper.ProjectTo<ContractDto>(contracts));
        }

        [HttpGet]
        [Route("getAllByEmployeeId/{employeeId}")]
        public IActionResult GetAllByEmployeeId(string employeeId)
        {
            var contracts = _contractRepository.GetAllByEmployeeId(employeeId);
            if (contracts == null || contracts.Count() == 0)
            {
                return Ok(new List<ApplicationUserDto>());
            }
            return Ok(_mapper.ProjectTo<ContractDto>(contracts));
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateContract([FromBody] ContractDto contractDto)
        {
            Contract toAdd = _mapper.Map<Contract>(contractDto);

            toAdd.Employee = await _accountService.GetChosenEmployee();
            if(toAdd.Employee == null) {
                return BadRequest("Could not find an employee to take the order.");
            }
            toAdd.EmployeeId = toAdd.Employee.Id;
            toAdd.Status = ContractStatus.Received;
            toAdd.StartDate = DateTime.Now;

            _contractRepository.Create(toAdd);
            bool result = _contractRepository.Save();
            if (!result)
            {
                return new StatusCodeResult(500);
            }
            //return Ok(_mapper.Map<ContractDto>(toAdd));
            return CreatedAtRoute("GetSingleContractById", new { id = toAdd.Id }, _mapper.Map<ContractDto>(toAdd));
        }

        [Authorize(Roles = Role.Employee)]
        [HttpPut]
        [Route("update")]
        public IActionResult UpdateContract([FromBody] UpdateContractDto updateContractDto)
        {
            Contract existingContract = _contractRepository.GetById(updateContractDto.Id);
            if (existingContract == null)
            {
                return NotFound();
            }
            _mapper.Map(updateContractDto, existingContract);
            _contractRepository.Update(existingContract);
            bool result = _contractRepository.Save();
            if (!result)
            {
                return new StatusCodeResult(500);
            }
            return Ok(_mapper.Map<ContractDto>(existingContract));
        }
    }
}
