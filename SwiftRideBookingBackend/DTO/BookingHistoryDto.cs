using System;

namespace SwiftRideBookingBackend.DTO
{
    public class BookingHistoryDto
    {
        public int BookingId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; }
    }
}
