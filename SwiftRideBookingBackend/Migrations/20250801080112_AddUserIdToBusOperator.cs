using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftRideBookingBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToBusOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add UserId column as nullable
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BusOperators",
                type: "int",
                nullable: true);

            // Step 2: Create index for UserId
            migrationBuilder.CreateIndex(
                name: "IX_BusOperators_UserId",
                table: "BusOperators",
                column: "UserId");

            // Step 3: Add foreign key constraint to Users
            migrationBuilder.AddForeignKey(
                name: "FK_BusOperators_Users_UserId",
                table: "BusOperators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusOperators_Users_UserId",
                table: "BusOperators");

            migrationBuilder.DropIndex(
                name: "IX_BusOperators_UserId",
                table: "BusOperators");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BusOperators");
        }
    }
}
