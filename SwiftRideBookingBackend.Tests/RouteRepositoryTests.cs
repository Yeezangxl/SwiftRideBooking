using NUnit.Framework;
using SwiftRideBookingBackend.Models;
using SwiftRideBookingBackend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SwiftRideBookingBackend.Tests
{
    [TestFixture]
    public class RouteRepositoryTests
    {
        private BookingContext _context;
        private RouteRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Route")
                .Options;

            _context = new BookingContext(options);
            _repo = new RouteRepository(_context);
        }

        [Test]
        public async Task AddRouteAsync_ShouldAddRoute()
        {
            var route = new Route { Origin = "Chennai", Destination = "Madurai" };
            await _repo.AddRouteAsync(route);
            await _repo.SaveAsync();

            var routes = await _repo.GetAllRoutesAsync();

            Assert.That(routes, Has.Exactly(1).Matches<Route>(r => r.Origin == "Chennai" && r.Destination == "Madurai"));
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
