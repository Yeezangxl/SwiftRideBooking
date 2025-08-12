using System;

namespace SwiftRideBookingBackend.Exceptions
{
    public class BookingNotFoundException : Exception
    {
        public BookingNotFoundException(string? message) : base(message) { }
    }
}
