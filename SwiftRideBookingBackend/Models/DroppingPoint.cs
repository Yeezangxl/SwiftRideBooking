namespace SwiftRideBookingBackend.Models
{
    public class DroppingPoint
    {
        public int DroppingPointId { get; set; }
        public string Point { get; set; }
        public int RouteId { get; set; }
        public virtual SwiftRideBookingBackend.Models.Route Route { get; set; }

    }
}
