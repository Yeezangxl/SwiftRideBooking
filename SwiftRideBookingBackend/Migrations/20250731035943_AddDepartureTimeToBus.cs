using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftRideBookingBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartureTimeToBus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Commented out because PasswordHash -> Password rename is NOT needed and causes errors!
            // migrationBuilder.RenameColumn(
            //     name: "PasswordHash",
            //     table: "Users",
            //     newName: "Password");

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureTime",
                table: "Buses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureTime",
                table: "Buses");

            // Commented out because Password -> PasswordHash rename is NOT needed!
            // migrationBuilder.RenameColumn(
            //     name: "Password",
            //     table: "Users",
            //     newName: "PasswordHash");
        }
    }
}
