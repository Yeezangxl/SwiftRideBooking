using System;
using System.Collections.Generic;

namespace SwiftRideBookingBackend.Models
{
    public class Bus
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }

        // For Operator Support
        public int? BusOperatorId { get; set; }
        public virtual BusOperator? BusOperator { get; set; }

        // Use this as requested!
        public virtual Route? Route { get; set; }

        public virtual ICollection<BusSeat> Seats { get; set; }
        public virtual ICollection<BusAmenity> BusAmenities { get; set; }
        public virtual ICollection<BusDeparture> Departures { get; set; }
    }
}
