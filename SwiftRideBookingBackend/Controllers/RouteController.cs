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

    public class RouteController : ControllerBase
    {
        private readonly IRouteRepository _routeRepo;
        private readonly BookingContext _context;
        private readonly IMapper _mapper;

        public RouteController(IRouteRepository routeRepo, BookingContext context, IMapper mapper)
        {
            _routeRepo = routeRepo;
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Route
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllRoutes()
        {
            var routes = await _routeRepo.GetAllRoutesAsync();
            return Ok(_mapper.Map<IEnumerable<RouteDto>>(routes));
        }

        // GET: api/Route/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetRouteById(int id)
        {
            var route = await _routeRepo.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();
            return Ok(_mapper.Map<RouteDto>(route));
        }

        // POST: api/Route
        [HttpPost]
        public async Task<IActionResult> AddRoute([FromBody] RouteDto routeDto)
        {
            var route = new SwiftRideBookingBackend.Models.Route
            {
                Origin = routeDto.Origin,
                Destination = routeDto.Destination
            };
            await _routeRepo.AddRouteAsync(route);
            await _routeRepo.SaveAsync();
            return Ok(_mapper.Map<RouteDto>(route));
        }

        // PUT: api/Route/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] RouteDto routeDto)
        {
            var route = await _routeRepo.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            route.Origin = routeDto.Origin;
            route.Destination = routeDto.Destination;

            await _routeRepo.SaveAsync();
            return Ok(_mapper.Map<RouteDto>(route));
        }

        // DELETE: api/Route/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _routeRepo.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            _context.Routes.Remove(route);
            await _routeRepo.SaveAsync();
            return Ok(new { Message = "Route deleted successfully." });
        }
        // GET: api/Route/locations
        [HttpGet("locations")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllLocations()
        {
            var routes = await _routeRepo.GetAllRoutesAsync();
            var locations = new HashSet<string>();

            foreach (var r in routes)
            {
                if (!string.IsNullOrEmpty(r.Origin))
                    locations.Add(r.Origin);
                if (!string.IsNullOrEmpty(r.Destination))
                    locations.Add(r.Destination);
            }
            return Ok(locations);
        }
        // POST: api/Route/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchRoutes([FromBody] SearchDto search)
        {
            var routes = await _routeRepo.GetAllRoutesAsync();
            var filtered = routes
                .Where(r => r.Origin == search.From && r.Destination == search.To)
                .ToList();
            // Add date/time logic as needed!
            return Ok(_mapper.Map<IEnumerable<RouteDto>>(filtered));
        }

    }
}
