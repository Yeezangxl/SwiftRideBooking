using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SwiftRideBookingBackend.DTO;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly BookingContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsersController(IUserRepository userRepo, BookingContext context, IMapper mapper, IConfiguration config)
        {
            _userRepo = userRepo;
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userRepo.GetAllUsersAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(_mapper.Map<UserDto>(user));
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.Role = userDto.Role;
            // Note: Password should be handled with care.

            await _userRepo.SaveAsync();
            return Ok(_mapper.Map<UserDto>(user));
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _userRepo.SaveAsync();
            return Ok(new { Message = "User deleted successfully." });
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAuthDto dto)
        {
            var existing = await _userRepo.GetUserByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest("User already exists!");

            var user = new User
            {
                Name = dto.Email.Split('@')[0],
                Email = dto.Email,
                Password = dto.Password, // 🟢 NO hashing, store as plain text!
                Role = "User"
            };
            await _userRepo.AddUserAsync(user);
            await _userRepo.SaveAsync();
            return Ok(_mapper.Map<UserDto>(user));
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAuthDto dto)
        {
            var user = await _userRepo.GetUserByEmailAsync(dto.Email);
            if (user == null || dto.Password != user.Password)
                return Unauthorized("Invalid credentials!");

            int? busOperatorId = null;

            // If logging in as BusOperator, fetch BusOperatorId
            if (user.Role == "BusOperator")
            {
                var busOp = await _context.BusOperators
                    .FirstOrDefaultAsync(b => b.UserId == user.UserId);
                if (busOp != null)
                    busOperatorId = busOp.BusOperatorId;
            }

            var token = GenerateJwtToken(user, busOperatorId);

            // Return busOperatorId as part of user object (frontend will store)
            var userObj = new
            {
                userId = user.UserId,
                name = user.Name,
                email = user.Email,
                role = user.Role,
                busOperatorId = busOperatorId
            };

            return Ok(new { token, user = userObj });
        }

        // Update: Accept BusOperatorId as extra claim if present
        private string GenerateJwtToken(User user, int? busOperatorId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["Jwt:Key"] ?? "your-256-bit-secret-should-be-very-long"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

            if (busOperatorId.HasValue)
            {
                claims.Add(new Claim("BusOperatorId", busOperatorId.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: System.DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
