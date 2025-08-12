using Microsoft.AspNetCore.Mvc;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftRideBookingBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace SwiftRideBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BusOperatorController : ControllerBase
    {
        private readonly IBusOperatorRepository _opRepo;
        private readonly BookingContext _context;
        private readonly IMapper _mapper;

        public BusOperatorController(IBusOperatorRepository opRepo, BookingContext context, IMapper mapper)
        {
            _opRepo = opRepo;
            _context = context;
            _mapper = mapper;
        }

        // GET: api/BusOperator
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusOperatorDto>>> GetAllOperators()
        {
            var ops = await _opRepo.GetAllBusOperatorsAsync();
            return Ok(_mapper.Map<IEnumerable<BusOperatorDto>>(ops));
        }

        // GET: api/BusOperator/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BusOperatorDto>> GetById(int id)
        {
            var op = await _opRepo.GetBusOperatorByIdAsync(id);
            if (op == null)
                return NotFound();
            return Ok(_mapper.Map<BusOperatorDto>(op));
        }

        // POST: api/BusOperator
        [HttpPost]
        public async Task<IActionResult> AddOperator([FromBody] BusOperatorDto dto)
        {
            // 1. Check if BusOperator exists
            var existingOp = await _opRepo.GetBusOperatorByEmailAsync(dto.Email);
            if (existingOp != null)
                return BadRequest("BusOperator already exists!");

            // 2. Check if user exists for login
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return BadRequest("User with this email already exists!");

            // 3. Add to BusOperators
            var op = new BusOperator
            {
                Name = dto.Name,
                Email = dto.Email
            };
            await _opRepo.AddBusOperatorAsync(op);

            // 4. Add to Users (for login)
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password, // just a password, no hash for you!
                Role = "BusOperator"
            };
            await _context.Users.AddAsync(user);

            await _opRepo.SaveAsync();
            return Ok(_mapper.Map<BusOperatorDto>(op));
        }

        // PUT: api/BusOperator/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOperator(int id, [FromBody] BusOperatorDto dto)
        {
            var op = await _opRepo.GetBusOperatorByIdAsync(id);
            if (op == null)
                return NotFound();

            op.Name = dto.Name;
            op.Email = dto.Email;
            await _opRepo.SaveAsync();
            return Ok(_mapper.Map<BusOperatorDto>(op));
        }

        // DELETE: api/BusOperator/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperator(int id)
        {
            var op = await _opRepo.GetBusOperatorByIdAsync(id);
            if (op == null)
                return NotFound();

            _context.BusOperators.Remove(op);

            // Optional: Remove from Users table also
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == op.Email && u.Role == "BusOperator");
            if (user != null)
                _context.Users.Remove(user);

            await _opRepo.SaveAsync();
            return Ok(new { Message = "BusOperator deleted successfully." });
        }
    }
}
