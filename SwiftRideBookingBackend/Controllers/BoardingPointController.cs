using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftRideBookingBackend.DTO;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize(Roles = "Admin,BusOperator")]
    public class BoardingPointController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly IMapper _mapper;

        public BoardingPointController(BookingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST: api/BoardingPoint

        [HttpPost]
        public async Task<IActionResult> AddBoardingPoint([FromBody] BoardingPointDto dto)
        {
            var bp = new BoardingPoint
            {
                Point = dto.Point,
                RouteId = dto.RouteId
            };
            await _context.BoardingPoints.AddAsync(bp);
            await _context.SaveChangesAsync();
            return Ok(bp);
        }

        // GET: api/BoardingPoint/route/1
        [HttpGet("route/{routeId}")]

        public async Task<ActionResult<IEnumerable<BoardingPoint>>> GetByRoute(int routeId)
        {
            var bps = await Task.FromResult(_context.BoardingPoints.Where(b => b.RouteId == routeId).ToList());
            return Ok(bps);
        }
    }
}
