namespace SwiftRideBookingBackend.DTO
{
    public class BusSeatDto
    {
        public int BusSeatId { get; set; }
        public int BusId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsBooked { get; set; }
    }
}
