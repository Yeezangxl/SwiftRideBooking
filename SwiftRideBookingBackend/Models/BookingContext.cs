using Microsoft.EntityFrameworkCore;

namespace SwiftRideBookingBackend.Models
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }

        // DBSets (Tables)
        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BusSeat> BusSeats { get; set; }
        public DbSet<BookingHistory> BookingHistories { get; set; }
        public DbSet<BusOperator> BusOperators { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<BusAmenity> BusAmenities { get; set; }
        public DbSet<BusDeparture> BusDepartures { get; set; }
        public DbSet<BoardingPoint> BoardingPoints { get; set; }
        public DbSet<DroppingPoint> DroppingPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique constraint on Email for Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Bus to Route
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.Route)
                .WithMany(r => r.Buses)
                .HasForeignKey(b => b.RouteId);

            // Booking: User & Bus
            modelBuilder.Entity<Booking>()
                .HasOne(bk => bk.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(bk => bk.UserId);

            modelBuilder.Entity<Booking>()
                .HasOne(bk => bk.Bus)
                .WithMany()
                .HasForeignKey(bk => bk.BusId);

            // Booking: BoardingPoint & DroppingPoint (new)
            modelBuilder.Entity<Booking>()
                .HasOne(bk => bk.BoardingPoint)
                .WithMany()
                .HasForeignKey(bk => bk.BoardingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(bk => bk.DroppingPoint)
                .WithMany()
                .HasForeignKey(bk => bk.DroppingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            // BusSeat to Bus
            modelBuilder.Entity<BusSeat>()
                .HasOne(bs => bs.Bus)
                .WithMany(b => b.Seats)
                .HasForeignKey(bs => bs.BusId);

            // BusAmenity to Bus & Amenity
            modelBuilder.Entity<BusAmenity>()
                .HasOne(ba => ba.Bus)
                .WithMany(b => b.BusAmenities)
                .HasForeignKey(ba => ba.BusId);

            modelBuilder.Entity<BusAmenity>()
                .HasOne(ba => ba.Amenity)
                .WithMany()
                .HasForeignKey(ba => ba.AmenityId);

            // BusDeparture to Bus
            modelBuilder.Entity<BusDeparture>()
                .HasOne(bd => bd.Bus)
                .WithMany(b => b.Departures)
                .HasForeignKey(bd => bd.BusId);

            // BoardingPoint/DroppingPoint to Route
            modelBuilder.Entity<BoardingPoint>()
                .HasOne(bp => bp.Route)
                .WithMany(r => r.BoardingPoints)
                .HasForeignKey(bp => bp.RouteId);

            modelBuilder.Entity<DroppingPoint>()
                .HasOne(dp => dp.Route)
                .WithMany(r => r.DroppingPoints)
                .HasForeignKey(dp => dp.RouteId);

            // Seed initial data (optional, for quick demo)
            modelBuilder.Entity<Route>().HasData(
                new Route { RouteId = 1, Origin = "Chennai", Destination = "Coimbatore" },
                new Route { RouteId = 2, Origin = "Madurai", Destination = "Chennai" },
                new Route { RouteId = 3, Origin = "Salem", Destination = "Madurai" },
                new Route { RouteId = 4, Origin = "Chennai", Destination = "Trichy" },
                new Route { RouteId = 5, Origin = "Erode", Destination = "Chennai" }
            );
            // You can add seed for Buses, Users, BoardingPoints, DroppingPoints, etc. later!
        }
    }
}
