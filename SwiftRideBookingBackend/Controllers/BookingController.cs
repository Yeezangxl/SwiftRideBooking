using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.DTO;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly BookingContext _context;
        private readonly IMapper _mapper;

        public BookingController(IBookingRepository bookingRepo, BookingContext context, IMapper mapper)
        {
            _bookingRepo = bookingRepo;
            _context = context;
            _mapper = mapper;
        }

        // POST: api/Booking
        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] BookingDto bookingDto)
        {
            if (bookingDto == null || bookingDto.UserId <= 0 || bookingDto.BusId <= 0 || bookingDto.JourneyDate == default)
                return BadRequest("Invalid booking data!");

            var bus = await _context.Buses.FindAsync(bookingDto.BusId);
            if (bus == null)
                return BadRequest("Bus not found!");

            var booking = new Booking
            {
                UserId = bookingDto.UserId,
                BusId = bookingDto.BusId,
                JourneyDate = bookingDto.JourneyDate,
                BookingDate = System.DateTime.UtcNow,
                SeatNumbers = bookingDto.SeatNumbers ?? "A1",
                Status = "Booked",
                BoardingPointId = bookingDto.BoardingPointId,
                DroppingPointId = bookingDto.DroppingPointId
            };
            await _bookingRepo.AddBookingAsync(booking);
            await _bookingRepo.SaveAsync();

            return Ok(new { Message = "Booking Successful!", BookingId = booking.BookingId });
        }

        // GET: api/Booking/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser(int userId)
        {
            // Fetch with Bus and Route!
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Bus)
                    .ThenInclude(bus => bus.Route)
                .ToListAsync();

            // Map manually to include nested Bus & Route DTOs
            var dtos = bookings.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                BusId = b.BusId,
                JourneyDate = b.JourneyDate,
                SeatNumbers = b.SeatNumbers,
                BoardingPointId = b.BoardingPointId,
                DroppingPointId = b.DroppingPointId,
                Bus = b.Bus == null ? null : new BusDto
                {
                    BusId = b.Bus.BusId,
                    BusNumber = b.Bus.BusNumber,
                    BusType = b.Bus.BusType,
                    RouteId = b.Bus.RouteId,
                    DepartureTime = b.Bus.DepartureTime,
                    TotalSeats = b.Bus.TotalSeats,
                    Route = b.Bus.Route == null ? null : new RouteDto
                    {
                        RouteId = b.Bus.Route.RouteId,
                        Origin = b.Bus.Route.Origin,
                        Destination = b.Bus.Route.Destination
                    }
                }
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/Booking/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            var dto = new BookingDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                BusId = booking.BusId,
                JourneyDate = booking.JourneyDate,
                SeatNumbers = booking.SeatNumbers,
                BoardingPointId = booking.BoardingPointId,
                DroppingPointId = booking.DroppingPointId,
                Bus = booking.Bus == null ? null : new BusDto
                {
                    BusId = booking.Bus.BusId,
                    BusNumber = booking.Bus.BusNumber,
                    BusType = booking.Bus.BusType,
                    RouteId = booking.Bus.RouteId,
                    DepartureTime = booking.Bus.DepartureTime,
                    TotalSeats = booking.Bus.TotalSeats,
                    Route = booking.Bus.Route == null ? null : new RouteDto
                    {
                        RouteId = booking.Bus.Route.RouteId,
                        Origin = booking.Bus.Route.Origin,
                        Destination = booking.Bus.Route.Destination
                    }
                }
            };

            return Ok(dto);
        }

        // PUT: api/Booking/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto bookingDto)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            booking.BusId = bookingDto.BusId;
            booking.UserId = bookingDto.UserId;
            booking.JourneyDate = bookingDto.JourneyDate;
            booking.SeatNumbers = bookingDto.SeatNumbers ?? booking.SeatNumbers;
            booking.BoardingPointId = bookingDto.BoardingPointId;
            booking.DroppingPointId = bookingDto.DroppingPointId;

            await _bookingRepo.SaveAsync();
            return Ok(_mapper.Map<BookingDto>(booking));
        }

        // DELETE: api/Booking/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            _context.Bookings.Remove(booking);
            await _bookingRepo.SaveAsync();
            return Ok(new { Message = "Booking deleted successfully." });
        }

        // POST: api/Booking/search
        [HttpPost("search")]
        public async Task<IActionResult> SearchBuses([FromBody] SearchBusDto search)
        {
            if (search == null || string.IsNullOrWhiteSpace(search.From) || string.IsNullOrWhiteSpace(search.To) || search.Date == default)
                return BadRequest("Invalid search data!");

            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.Origin == search.From && r.Destination == search.To);

            if (route == null)
                return NotFound("No such route!");

            var dateOnly = search.Date.Date;
            var buses = await _context.Buses
                .Where(b => b.RouteId == route.RouteId && b.DepartureTime.Date == dateOnly)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BusDto>>(buses));
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Bus)
                    .ThenInclude(bus => bus.Route)
                .ToListAsync();

            var dtos = bookings.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                BusId = b.BusId,
                JourneyDate = b.JourneyDate,
                SeatNumbers = b.SeatNumbers,
                BoardingPointId = b.BoardingPointId,
                DroppingPointId = b.DroppingPointId,
                Bus = b.Bus == null ? null : new BusDto
                {
                    BusId = b.Bus.BusId,
                    BusNumber = b.Bus.BusNumber,
                    BusType = b.Bus.BusType,
                    RouteId = b.Bus.RouteId,
                    DepartureTime = b.Bus.DepartureTime,
                    TotalSeats = b.Bus.TotalSeats,
                    BusOperatorId = b.Bus.BusOperatorId,
                    Route = b.Bus.Route == null ? null : new RouteDto
                    {
                        RouteId = b.Bus.Route.RouteId,
                        Origin = b.Bus.Route.Origin,
                        Destination = b.Bus.Route.Destination
                    }
                }
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/Booking/bookedseats?busId=XX&date=YYYY-MM-DD
        [HttpGet("bookedseats")]
        public async Task<IActionResult> GetBookedSeats(int busId, DateTime date)
        {
            var bookedSeats = await _context.Bookings
                .Where(b => b.BusId == busId && b.JourneyDate.Date == date.Date && b.Status == "Booked")
                .Select(b => b.SeatNumbers)
                .ToListAsync();

            // Flatten comma-separated
            var seatList = bookedSeats
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

            return Ok(seatList);
        }

    }
}
