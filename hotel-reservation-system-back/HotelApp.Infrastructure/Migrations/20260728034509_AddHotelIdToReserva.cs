using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Reservas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HotelId",
                table: "Reservas",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Hoteis_HotelId",
                table: "Reservas",
                column: "HotelId",
                principalTable: "Hoteis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Hoteis_HotelId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HotelId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Reservas");
        }
    }
}
