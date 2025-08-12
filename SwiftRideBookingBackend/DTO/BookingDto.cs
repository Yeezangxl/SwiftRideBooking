using System;

namespace SwiftRideBookingBackend.DTO
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int BusId { get; set; }
        public DateTime JourneyDate { get; set; }
        public string SeatNumbers { get; set; }
        public int BoardingPointId { get; set; }
        public int DroppingPointId { get; set; }
        public BusDto? Bus { get; set; } // for bus info!
    }
}
