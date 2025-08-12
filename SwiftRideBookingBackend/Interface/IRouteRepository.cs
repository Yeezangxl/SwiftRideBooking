using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Interface
{
    public interface IRouteRepository
    {
        Task<IEnumerable<SwiftRideBookingBackend.Models.Route>> GetAllRoutesAsync();
        Task<SwiftRideBookingBackend.Models.Route> GetRouteByIdAsync(int id);
        Task AddRouteAsync(SwiftRideBookingBackend.Models.Route route);
        Task<bool> SaveAsync();
    }
}
