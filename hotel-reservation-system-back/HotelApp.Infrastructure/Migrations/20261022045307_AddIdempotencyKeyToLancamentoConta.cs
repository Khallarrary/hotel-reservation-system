using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyKeyToLancamentoConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChaveIdempotencia",
                table: "LancamentoConta",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoConta_ContaReservaId_ChaveIdempotencia",
                table: "LancamentoConta",
                columns: new[] { "ContaReservaId", "ChaveIdempotencia" },
                unique: true,
                filter: "\"ChaveIdempotencia\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LancamentoConta_ContaReservaId_ChaveIdempotencia",
                table: "LancamentoConta");

            migrationBuilder.DropColumn(
                name: "ChaveIdempotencia",
                table: "LancamentoConta");
        }
    }
}
