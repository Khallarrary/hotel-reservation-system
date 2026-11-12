using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservaHospede : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservaHospedes",
                columns: table => new
                {
                    ReservaId = table.Column<int>(type: "integer", nullable: false),
                    HospedeId = table.Column<int>(type: "integer", nullable: false),
                    HotelId = table.Column<int>(type: "integer", nullable: false),
                    Titular = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaHospedes", x => new { x.ReservaId, x.HospedeId });
                    table.ForeignKey(
                        name: "FK_ReservaHospedes_Hospedes_HospedeId",
                        column: x => x.HospedeId,
                        principalTable: "Hospedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservaHospedes_Hoteis_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservaHospedes_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservaHospedes_HospedeId",
                table: "ReservaHospedes",
                column: "HospedeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaHospedes_HotelId",
                table: "ReservaHospedes",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservaHospedes");
        }
    }
}
