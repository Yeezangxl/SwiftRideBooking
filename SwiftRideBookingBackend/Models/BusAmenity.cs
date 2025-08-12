namespace SwiftRideBookingBackend.Models
{
    public class BusAmenity
    {
        public int BusAmenityId { get; set; }
        public int BusId { get; set; }
        public int AmenityId { get; set; }
        public virtual Bus Bus { get; set; }
        public virtual Amenity Amenity { get; set; }
    }
}
