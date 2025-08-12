using NUnit.Framework;
using SwiftRideBookingBackend.Models;
using SwiftRideBookingBackend.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Tests
{
    [TestFixture]
    public class BookingRepositoryTests
    {
        private BookingContext _context;
        private BookingRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Booking")
                .Options;

            _context = new BookingContext(options);
            _repo = new BookingRepository(_context);

            // Add user and bus
            _context.Users.Add(new User { UserId = 1, Name = "Uma", Email = "uma@email.com", Password = "User@123", Role = "User" });
            _context.Buses.Add(new Bus { BusId = 1, BusNumber = "TN01", BusType = "AC", RouteId = 1, TotalSeats = 40 });
            _context.SaveChanges();
        }

        [Test]
        public async Task AddBookingAsync_ShouldAddBooking()
        {
            var booking = new Booking
            {
                UserId = 1,
                BusId = 1,
                BookingDate = DateTime.UtcNow,
                JourneyDate = DateTime.UtcNow.AddDays(1),
                SeatNumbers = "12,13",
                Status = "Booked"
            };
            await _repo.AddBookingAsync(booking);
            await _repo.SaveAsync();

            var bookings = await _repo.GetBookingsByUserAsync(1);

            Assert.That(bookings, Has.Exactly(1).Matches<Booking>(b => b.SeatNumbers == "12,13"));
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
