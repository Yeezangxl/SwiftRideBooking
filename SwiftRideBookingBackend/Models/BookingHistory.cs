using System;

namespace SwiftRideBookingBackend.Models
{
    public class BookingHistory
    {
        public int BookingHistoryId { get; set; }
        public int BookingId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; }
    }
}
