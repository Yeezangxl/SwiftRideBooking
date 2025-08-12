using System.Collections.Generic;
using System.Threading.Tasks;
using SwiftRideBookingBackend.Models;

namespace SwiftRideBookingBackend.Interface
{
    public interface IBusOperatorRepository
    {
        Task<IEnumerable<BusOperator>> GetAllBusOperatorsAsync();
        Task<BusOperator?> GetBusOperatorByIdAsync(int id);
        Task<BusOperator?> GetBusOperatorByEmailAsync(string email);
        Task AddBusOperatorAsync(BusOperator op);
        Task<bool> SaveAsync();
    }
}
