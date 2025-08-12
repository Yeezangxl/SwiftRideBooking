using System;

namespace SwiftRideBookingBackend.Exceptions
{
    public class RouteNotFoundException : Exception
    {
        public RouteNotFoundException(string? message) : base(message) { }
    }
}
