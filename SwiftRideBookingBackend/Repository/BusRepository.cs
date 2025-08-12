using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using SwiftRideBookingBackend.Exceptions; // Add this

namespace SwiftRideBookingBackend.Repository
{
    public class BusRepository : IBusRepository
    {
        private readonly BookingContext _context;
        public BusRepository(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bus>> GetAllBusesAsync()
        {
            return await _context.Buses.Include(b => b.Route).ToListAsync();
        }

        public async Task<Bus> GetBusByIdAsync(int id)
        {
            var bus = await _context.Buses.Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.BusId == id);
            if (bus == null)
                throw new BusNotFoundException($"Bus with ID {id} not found.");
            return bus;
        }

        public async Task<IEnumerable<Bus>> GetBusesByRouteAsync(int routeId)
        {
            return await _context.Buses.Where(b => b.RouteId == routeId)
                .Include(b => b.Route)
                .ToListAsync();
        }

        public async Task AddBusAsync(Bus bus)
        {
            await _context.Buses.AddAsync(bus);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
