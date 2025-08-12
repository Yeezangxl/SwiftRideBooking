using NUnit.Framework;
using SwiftRideBookingBackend.Models;
using SwiftRideBookingBackend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Tests
{
    [TestFixture]
    public class BusRepositoryTests
    {
        private BookingContext _context;
        private BusRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Bus")
                .Options;

            _context = new BookingContext(options);
            _repo = new BusRepository(_context);

            // Add a dummy route (bus needs a valid route)
            _context.Routes.Add(new Route { RouteId = 1, Origin = "A", Destination = "B" });
            _context.SaveChanges();
        }

        [Test]
        public async Task AddBusAsync_ShouldAddBus()
        {
            var bus = new Bus { BusNumber = "TN01", BusType = "AC", RouteId = 1, TotalSeats = 40 };
            await _repo.AddBusAsync(bus);
            await _repo.SaveAsync();

            var buses = await _repo.GetAllBusesAsync();
            Assert.That(buses, Has.Exactly(1).Matches<Bus>(b => b.BusNumber == "TN01"));
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
