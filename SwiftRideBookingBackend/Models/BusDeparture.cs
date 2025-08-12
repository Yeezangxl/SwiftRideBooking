using System;

namespace SwiftRideBookingBackend.Models
{
    public class BusDeparture
    {
        public int BusDepartureId { get; set; }
        public int BusId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public virtual Bus Bus { get; set; }
    }
}
