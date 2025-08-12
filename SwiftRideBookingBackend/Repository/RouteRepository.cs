using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using SwiftRideBookingBackend.Exceptions; // Add this

namespace SwiftRideBookingBackend.Repository
{
    public class RouteRepository : IRouteRepository
    {
        private readonly BookingContext _context;
        public RouteRepository(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SwiftRideBookingBackend.Models.Route>> GetAllRoutesAsync()
        {
            return await _context.Routes.ToListAsync();
        }

        public async Task<SwiftRideBookingBackend.Models.Route> GetRouteByIdAsync(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                throw new RouteNotFoundException($"Route with ID {id} not found.");
            return route;
        }

        public async Task AddRouteAsync(SwiftRideBookingBackend.Models.Route route)
        {
            await _context.Routes.AddAsync(route);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
