namespace SwiftRideBookingBackend.DTO
{
    public class BusDto
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
        public int? BusOperatorId { get; set; }
        public RouteDto? Route { get; set; } // <-- FIXED!
    }
}
