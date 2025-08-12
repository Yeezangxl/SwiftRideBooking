using SwiftRideBookingBackend.Models;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } // Full name
    public string Email { get; set; }
    public string Password { get; set; }
    // Can rename to Password if you want
    public string Role { get; set; } // "Admin", "User", "BusOperator"
    public virtual ICollection<Booking> Bookings { get; set; }
}
