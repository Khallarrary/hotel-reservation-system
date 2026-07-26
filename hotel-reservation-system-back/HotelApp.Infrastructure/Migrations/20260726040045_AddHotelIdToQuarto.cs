using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToQuarto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quartos_Numero",
                table: "Quartos");

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Quartos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_HotelId",
                table: "Usuario",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Quartos_HotelId_Numero",
                table: "Quartos",
                columns: new[] { "HotelId", "Numero" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quartos_Hoteis_HotelId",
                table: "Quartos",
                column: "HotelId",
                principalTable: "Hoteis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Hoteis_HotelId",
                table: "Usuario",
                column: "HotelId",
                principalTable: "Hoteis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quartos_Hoteis_HotelId",
                table: "Quartos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Hoteis_HotelId",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_HotelId",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Quartos_HotelId_Numero",
                table: "Quartos");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Quartos");

            migrationBuilder.CreateIndex(
                name: "IX_Quartos_Numero",
                table: "Quartos",
                column: "Numero",
                unique: true);
        }
    }
}
