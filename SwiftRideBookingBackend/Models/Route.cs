using System.Collections.Generic;

namespace SwiftRideBookingBackend.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public virtual ICollection<Bus> Buses { get; set; }
        public virtual ICollection<BoardingPoint> BoardingPoints { get; set; }
        public virtual ICollection<DroppingPoint> DroppingPoints { get; set; }
    }
}
