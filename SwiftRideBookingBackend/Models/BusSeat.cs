namespace SwiftRideBookingBackend.Models
{
    public class BusSeat
    {
        public int BusSeatId { get; set; }
        public int BusId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsBooked { get; set; }
        public virtual Bus Bus { get; set; }
    }
}
