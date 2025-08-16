using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwiftRideBookingBackend.DTO;
using SwiftRideBookingBackend.Interface;
using SwiftRideBookingBackend.Models;
using System;
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
                BookingDate = DateTime.UtcNow,
                SeatNumbers = bookingDto.SeatNumbers ?? "A1",
                Status = "Booked",
                BoardingPointId = bookingDto.BoardingPointId,
                DroppingPointId = bookingDto.DroppingPointId
            };

            await _bookingRepo.AddBookingAsync(booking);
            await _bookingRepo.SaveAsync();

            return Ok(new { Message = "Booking Successful!", BookingId = booking.BookingId, Status = booking.Status });
        }

        // === SOFT-CANCEL (keeps row; admin can still see it) ===
        // POST: api/Booking/{id}/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();

            if (!string.Equals(booking.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                booking.Status = "Cancelled";
                await _bookingRepo.SaveAsync();
            }

            return Ok(new { Message = "Booking cancelled.", BookingId = booking.BookingId, Status = booking.Status });
        }

        // GET: api/Booking/user/{userId}
        // Return anonymous objects that include Status (no need to modify BookingDto)
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUser(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Bus)!.ThenInclude(bus => bus.Route)
                .ToListAsync();

            var result = bookings.Select(b => new
            {
                b.BookingId,
                b.UserId,
                b.BusId,
                b.JourneyDate,
                b.SeatNumbers,
                b.BoardingPointId,
                b.DroppingPointId,
                Status = b.Status,
                Bus = b.Bus == null ? null : new
                {
                    b.Bus.BusId,
                    b.Bus.BusNumber,
                    b.Bus.BusType,
                    b.Bus.RouteId,
                    b.Bus.DepartureTime,
                    b.Bus.TotalSeats,
                    Route = b.Bus.Route == null ? null : new
                    {
                        b.Bus.Route.RouteId,
                        b.Bus.Route.Origin,
                        b.Bus.Route.Destination
                    }
                }
            });

            return Ok(result);
        }

        // GET: api/Booking/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var b = await _context.Bookings
                .Include(x => x.Bus)!.ThenInclude(bus => bus.Route)
                .FirstOrDefaultAsync(x => x.BookingId == id);

            if (b == null) return NotFound();

            var result = new
            {
                b.BookingId,
                b.UserId,
                b.BusId,
                b.JourneyDate,
                b.SeatNumbers,
                b.BoardingPointId,
                b.DroppingPointId,
                Status = b.Status,
                Bus = b.Bus == null ? null : new
                {
                    b.Bus.BusId,
                    b.Bus.BusNumber,
                    b.Bus.BusType,
                    b.Bus.RouteId,
                    b.Bus.DepartureTime,
                    b.Bus.TotalSeats,
                    Route = b.Bus.Route == null ? null : new
                    {
                        b.Bus.Route.RouteId,
                        b.Bus.Route.Origin,
                        b.Bus.Route.Destination
                    }
                }
            };

            return Ok(result);
        }

        // PUT: api/Booking/{id}  (no Status here because BookingDto doesn’t have it)
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

        // DELETE: api/Booking/{id}  (hard delete; keep for admin if you want)
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

        // GET: api/Booking  (admin list; includes Status)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Bus)!.ThenInclude(bus => bus.Route)
                .ToListAsync();

            var result = bookings.Select(b => new
            {
                b.BookingId,
                b.UserId,
                b.BusId,
                b.JourneyDate,
                b.SeatNumbers,
                b.BoardingPointId,
                b.DroppingPointId,
                Status = b.Status,
                Bus = b.Bus == null ? null : new
                {
                    b.Bus.BusId,
                    b.Bus.BusNumber,
                    b.Bus.BusType,
                    b.Bus.RouteId,
                    b.Bus.DepartureTime,
                    b.Bus.TotalSeats,
                    b.Bus.BusOperatorId,
                    Route = b.Bus.Route == null ? null : new
                    {
                        b.Bus.Route.RouteId,
                        b.Bus.Route.Origin,
                        b.Bus.Route.Destination
                    }
                }
            });

            return Ok(result);
        }

        // GET: api/Booking/bookedseats?busId=XX&date=YYYY-MM-DD
        [HttpGet("bookedseats")]
        public async Task<IActionResult> GetBookedSeats(int busId, DateTime date)
        {
            // FIX: use b.Status (capital S)
            var bookedSeats = await _context.Bookings
                .Where(b => b.BusId == busId
                         && b.JourneyDate.Date == date.Date
                         && b.Status == "Booked")
                .Select(b => b.SeatNumbers)
                .ToListAsync();

            var seatList = bookedSeats
                .SelectMany(s => s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

            return Ok(seatList);
        }
    }
}
