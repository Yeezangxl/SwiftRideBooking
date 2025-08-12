using System;

namespace SwiftRideBookingBackend.DTO
{
    public class BusDepartureDto
    {
        public int BusDepartureId { get; set; }
        public int BusId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
