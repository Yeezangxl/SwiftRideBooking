using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using SwiftRideBookingBackend.Exceptions; // Add this

namespace SwiftRideBookingBackend.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingContext _context;
        public BookingRepository(BookingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Bus)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Bus)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
                throw new BookingNotFoundException($"Booking with ID {bookingId} not found.");
            return booking;
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
