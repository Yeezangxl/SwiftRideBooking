using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;

namespace SwiftRideBookingBackend.Repository
{
    public class BusOperatorRepository : IBusOperatorRepository
    {
        private readonly BookingContext _context;
        public BusOperatorRepository(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BusOperator>> GetAllBusOperatorsAsync()
        {
            return await _context.BusOperators.ToListAsync();
        }

        public async Task<BusOperator?> GetBusOperatorByIdAsync(int id)
        {
            return await _context.BusOperators.FindAsync(id);
        }

        public async Task<BusOperator?> GetBusOperatorByEmailAsync(string email)
        {
            return await _context.BusOperators.FirstOrDefaultAsync(b => b.Email == email);
        }

        public async Task AddBusOperatorAsync(BusOperator op)
        {
            await _context.BusOperators.AddAsync(op);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
