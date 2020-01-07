using AutoMapper;
using GSM.Dtos;
using GSM.Models;
using GSM.Repositories;
using GSM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GadgetController : ControllerBase
    {
        private readonly IGadgetRepository _gadgetRepository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public GadgetController(IGadgetRepository gadgetRepository, IMapper mapper, IAccountService accountService)
        {
            _gadgetRepository = gadgetRepository;
            _mapper = mapper;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("getById/{id}", Name = "GetSingleGadgetById")]
        public IActionResult GetSingleGadgetById(string id)
        {
            Gadget gadget = _gadgetRepository.GetById(id);
            if (gadget == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<GadgetDto>(gadget));
        }

        [Authorize(Roles = Role.Employee)]
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var gadgets = _gadgetRepository.GetAll();
            if (gadgets == null)
            {
                return Ok(new List<GadgetDto>());
            }
            return Ok(gadgets);
        }

        [HttpGet]
        [Route("getAllForCustomer")]
        public async Task<IActionResult> GetAllForCustomer()
        {
            var currentUser = await _accountService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();
            IQueryable<Gadget> gadgets = _gadgetRepository.GetAllByCustomerId(currentUser.Id);
            if (gadgets == null || gadgets.Count() == 0)
            {
                return Ok(new List<ApplicationUserDto>());
            }
            return Ok(_mapper.ProjectTo<GadgetDto>(gadgets));
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateGadget([FromBody] GadgetDto gadgetDto)
        {
            Gadget toAdd = _mapper.Map<Gadget>(gadgetDto);
            var currentUser = await _accountService.GetCurrentUserAsync();
            if (currentUser == null)
                return Unauthorized();
            toAdd.CustomerId = currentUser.Id;
            _gadgetRepository.Create(toAdd);
            bool result = _gadgetRepository.Save();
            if (!result)
            {
                return new StatusCodeResult(500);
            }
            //return Ok(_mapper.Map<GadgetDto>(toAdd));
            return CreatedAtRoute("GetSingleGadgetById", new { id = toAdd.Id }, _mapper.Map<GadgetDto>(toAdd));
        }

        [Authorize(Roles = Role.Customer)]
        [HttpPut]
        [Route("update")]
        public IActionResult UpdateGadget([FromBody] GadgetDto gadgetDto)
        {
            Gadget existingGadget = _gadgetRepository.GetById(gadgetDto.Id);
            if (existingGadget == null)
            {
                return NotFound();
            }
            _mapper.Map(gadgetDto, existingGadget);
            _gadgetRepository.Update(existingGadget);
            bool result = _gadgetRepository.Save();
            if (!result)
            {
                return new StatusCodeResult(500);
            }
            return Ok(_mapper.Map<ContractDto>(existingGadget));
        }
    }
}
