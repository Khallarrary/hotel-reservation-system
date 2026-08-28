using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyKeyToReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_HotelId",
                table: "Reservas");

            migrationBuilder.AddColumn<Guid>(
                name: "ChaveIdempotencia",
                table: "Reservas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HotelId_ChaveIdempotencia",
                table: "Reservas",
                columns: new[] { "HotelId", "ChaveIdempotencia" },
                unique: true,
                filter: "\"ChaveIdempotencia\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_HotelId_ChaveIdempotencia",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ChaveIdempotencia",
                table: "Reservas");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HotelId",
                table: "Reservas",
                column: "HotelId");
        }
    }
}
