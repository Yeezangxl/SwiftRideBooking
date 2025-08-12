using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftRideBookingBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardingPointId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DroppingPointId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BoardingPointId",
                table: "Bookings",
                column: "BoardingPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DroppingPointId",
                table: "Bookings",
                column: "DroppingPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BoardingPoints_BoardingPointId",
                table: "Bookings",
                column: "BoardingPointId",
                principalTable: "BoardingPoints",
                principalColumn: "BoardingPointId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_DroppingPoints_DroppingPointId",
                table: "Bookings",
                column: "DroppingPointId",
                principalTable: "DroppingPoints",
                principalColumn: "DroppingPointId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BoardingPoints_BoardingPointId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_DroppingPoints_DroppingPointId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BoardingPointId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DroppingPointId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BoardingPointId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DroppingPointId",
                table: "Bookings");
        }
    }
}
