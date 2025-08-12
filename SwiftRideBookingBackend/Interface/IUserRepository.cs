using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftRideBookingBackend.Models;

namespace SwiftRideBookingBackend.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<bool> SaveAsync();
    }
}
