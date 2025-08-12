namespace SwiftRideBookingBackend.Models
{
    public class BusOperator
    {
        public int BusOperatorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // 🔥 Add this line to link to User
        public int UserId { get; set; }
        public User User { get; set; } // Optional, for EF navigation
    }
}
