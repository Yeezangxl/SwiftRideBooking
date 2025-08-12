using System;

namespace SwiftRideBookingBackend.Exceptions
{
    public class BusNotFoundException : Exception
    {
        public BusNotFoundException(string? message) : base(message) { }
    }
}
