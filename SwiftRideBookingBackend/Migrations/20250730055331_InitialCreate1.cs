using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftRideBookingBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "BusOperators");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "BusOperators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
