namespace SwiftRideBookingBackend.Models
{
    public class BoardingPoint
    {
        public int BoardingPointId { get; set; }
        public string Point { get; set; }
        public int RouteId { get; set; }
        public virtual SwiftRideBookingBackend.Models.Route Route { get; set; }

    }
}
