// DTO/SearchBusDto.cs
using System;

namespace SwiftRideBookingBackend.DTO
{
    public class SearchBusDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
    }
}
