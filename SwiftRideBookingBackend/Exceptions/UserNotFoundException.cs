using System;

namespace SwiftRideBookingBackend.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string? message) : base(message) { }
    }
}
