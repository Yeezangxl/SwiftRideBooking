using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.DTO;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace SwiftRideBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusController : ControllerBase
    {
        private readonly IBusRepository _busRepo;
        private readonly BookingContext _context;
        private readonly IMapper _mapper;

        public BusController(IBusRepository busRepo, BookingContext context, IMapper mapper)
        {
            _busRepo = busRepo;
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Bus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusDto>>> GetAllBuses()
        {
            var buses = await _context.Buses
                .Include(b => b.Route)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<BusDto>>(buses));
        }

        // GET: api/Bus/route/{routeId}
        [HttpGet("route/{routeId}")]
        public async Task<ActionResult<IEnumerable<BusDto>>> GetBusesByRoute(int routeId)
        {
            var buses = await _context.Buses
                .Include(b => b.Route)
                .Where(b => b.RouteId == routeId)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<BusDto>>(buses));
        }

        // GET: api/Bus/operator/{operatorId}
        [HttpGet("operator/{operatorId}")]
        [Authorize(Roles = "BusOperator,Admin")]
        public async Task<ActionResult<IEnumerable<BusDto>>> GetBusesByOperator(int operatorId)
        {
            // Optionally, check BusOperatorId from JWT and compare to operatorId for extra security
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var jwtOperatorId = User.Claims.FirstOrDefault(c => c.Type == "BusOperatorId")?.Value;

            if (userRole == "BusOperator" && jwtOperatorId != null && jwtOperatorId != operatorId.ToString())
                return Forbid();

            var buses = await _context.Buses
                .Include(b => b.Route)
                .Where(b => b.BusOperatorId == operatorId)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BusDto>>(buses));
        }

        // POST: api/Bus
        [HttpPost]
        [Authorize(Roles = "Admin,BusOperator")]
        public async Task<IActionResult> AddBus([FromBody] BusDto busDto)
        {
            // Find role and BusOperatorId from claims
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var operatorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "BusOperatorId")?.Value;

            if (userRole == "BusOperator" && int.TryParse(operatorIdClaim, out int operatorId))
            {
                busDto.BusOperatorId = operatorId; // Always set for operator
            }

            // Optional: For Admin, ensure BusOperatorId is provided
            if (userRole == "Admin" && busDto.BusOperatorId == 0)
            {
                return BadRequest("Admin must specify BusOperatorId.");
            }

            var bus = _mapper.Map<Bus>(busDto);
            await _busRepo.AddBusAsync(bus);
            await _busRepo.SaveAsync();
            return Ok(_mapper.Map<BusDto>(bus));
        }

        // GET: api/Bus/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BusDto>> GetBusById(int id)
        {
            var bus = await _context.Buses
                .Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.BusId == id);
            if (bus == null)
                return NotFound();
            return Ok(_mapper.Map<BusDto>(bus));
        }

        // PUT: api/Bus/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,BusOperator")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] BusDto busDto)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusId == id);
            if (bus == null)
                return NotFound();

            bus.BusNumber = busDto.BusNumber;
            bus.BusType = busDto.BusType;
            bus.RouteId = busDto.RouteId;
            bus.TotalSeats = busDto.TotalSeats;
            bus.DepartureTime = busDto.DepartureTime;

            // Only Admins can change BusOperatorId (recommended for safety)
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (userRole == "Admin")
            {
                bus.BusOperatorId = busDto.BusOperatorId;
            }

            await _busRepo.SaveAsync();
            return Ok(_mapper.Map<BusDto>(bus));
        }

        // DELETE: api/Bus/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,BusOperator")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusId == id);
            if (bus == null)
                return NotFound();

            _context.Buses.Remove(bus);
            await _busRepo.SaveAsync();
            return Ok(new { Message = "Bus deleted successfully." });
        }

        // POST: api/Bus/search
        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchBuses([FromBody] SearchBusDto search)
        {
            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.Origin == search.From && r.Destination == search.To);

            if (route == null)
                return NotFound("No such route!");

            var dateOnly = search.Date.Date;
            var buses = await _context.Buses
                .Include(b => b.Route)
                .Where(b => b.RouteId == route.RouteId && b.DepartureTime.Date == dateOnly)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BusDto>>(buses));
        }

        // GET: api/Bus/boardingpoints?routeId=1
        [HttpGet("boardingpoints")]
        public async Task<IActionResult> GetBoardingPoints(int routeId)
        {
            var points = await _context.BoardingPoints
                .Where(bp => bp.RouteId == routeId)
                .Select(bp => new { bp.BoardingPointId, bp.Point })
                .ToListAsync();

            return Ok(points);
        }

        // GET: api/Bus/droppingpoints?routeId=1
        [HttpGet("droppingpoints")]
        public async Task<IActionResult> GetDroppingPoints(int routeId)
        {
            var points = await _context.DroppingPoints
                .Where(dp => dp.RouteId == routeId)
                .Select(dp => new { dp.DroppingPointId, dp.Point })
                .ToListAsync();

            return Ok(points);
        }

        // POST: api/Bus/boardingpoints
        [HttpPost("boardingpoints")]
        [Authorize(Roles = "Admin,BusOperator")]
        public async Task<IActionResult> AddBoardingPoint([FromBody] BoardingPointDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Point) || dto.RouteId == 0)
                return BadRequest("Point and RouteId required");

            var newPoint = new BoardingPoint
            {
                Point = dto.Point,
                RouteId = dto.RouteId
            };
            _context.BoardingPoints.Add(newPoint);
            await _context.SaveChangesAsync();

            return Ok(new { newPoint.BoardingPointId, newPoint.Point });
        }

        // POST: api/Bus/droppingpoints
        [HttpPost("droppingpoints")]
        [Authorize(Roles = "Admin,BusOperator")]
        public async Task<IActionResult> AddDroppingPoint([FromBody] DroppingPointDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Point) || dto.RouteId == 0)
                return BadRequest("Point and RouteId required");

            var newPoint = new DroppingPoint
            {
                Point = dto.Point,
                RouteId = dto.RouteId
            };
            _context.DroppingPoints.Add(newPoint);
            await _context.SaveChangesAsync();

            return Ok(new { newPoint.DroppingPointId, newPoint.Point });
        }
    }
}
