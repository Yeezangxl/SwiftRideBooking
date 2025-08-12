using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftRideBookingBackend.Models;

namespace SwiftRideBookingBackend.Interface
{
    public interface IBusRepository
    {
        Task<IEnumerable<Bus>> GetAllBusesAsync();
        Task<Bus> GetBusByIdAsync(int id);
        Task<IEnumerable<Bus>> GetBusesByRouteAsync(int routeId);
        Task AddBusAsync(Bus bus);
        Task<bool> SaveAsync();
    }
}
