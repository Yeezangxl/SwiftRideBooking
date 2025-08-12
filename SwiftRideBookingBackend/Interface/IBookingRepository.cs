using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftRideBookingBackend.Models;

namespace SwiftRideBookingBackend.Interface
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task AddBookingAsync(Booking booking);
        Task<bool> SaveAsync();
    }
}
