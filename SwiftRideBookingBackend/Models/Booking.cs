using System;
using System.Collections.Generic;

namespace SwiftRideBookingBackend.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int BusId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime JourneyDate { get; set; }
        public string SeatNumbers { get; set; } // e.g., "A1,A2,B3"
        public string Status { get; set; } // "Booked", "Cancelled"

        // New fields:
        public int BoardingPointId { get; set; }
        public int DroppingPointId { get; set; }
        public virtual BoardingPoint BoardingPoint { get; set; }
        public virtual DroppingPoint DroppingPoint { get; set; }

        public virtual User User { get; set; }
        public virtual Bus Bus { get; set; }
    }
}
