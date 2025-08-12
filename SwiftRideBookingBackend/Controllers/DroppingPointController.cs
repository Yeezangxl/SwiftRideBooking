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

    public class DroppingPointController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly IMapper _mapper;

        public DroppingPointController(BookingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST: api/DroppingPoint
        [HttpPost]
        public async Task<IActionResult> AddDroppingPoint([FromBody] DroppingPointDto dto)
        {
            var dp = new DroppingPoint
            {
                Point = dto.Point,
                RouteId = dto.RouteId
            };
            await _context.DroppingPoints.AddAsync(dp);
            await _context.SaveChangesAsync();
            return Ok(dp);
        }

        // GET: api/DroppingPoint/route/1
        [HttpGet("route/{routeId}")]

   
        public async Task<ActionResult<IEnumerable<DroppingPoint>>> GetByRoute(int routeId)
        {
            var dps = await Task.FromResult(_context.DroppingPoints.Where(d => d.RouteId == routeId).ToList());
            return Ok(dps);
        }
    }
}
