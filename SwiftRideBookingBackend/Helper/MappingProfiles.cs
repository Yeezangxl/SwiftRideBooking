using AutoMapper;
using SwiftRideBookingBackend.DTO;
using SwiftRideBookingBackend.Models;
using Route = SwiftRideBookingBackend.Models.Route;

namespace SwiftRideBookingBackend.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Bus, BusDto>().ReverseMap();
            CreateMap<Route, RouteDto>().ReverseMap();
            CreateMap<Booking, BookingDto>().ReverseMap();
            CreateMap<BookingHistory, BookingHistoryDto>().ReverseMap();
            CreateMap<BusOperator, BusOperatorDto>().ReverseMap();
            CreateMap<Amenity, AmenityDto>().ReverseMap();
            CreateMap<BusSeat, BusSeatDto>().ReverseMap();
            CreateMap<BusDeparture, BusDepartureDto>().ReverseMap();
            CreateMap<BoardingPoint, BoardingPointDto>().ReverseMap();
            CreateMap<DroppingPoint, DroppingPointDto>().ReverseMap();
        }
    }
}
