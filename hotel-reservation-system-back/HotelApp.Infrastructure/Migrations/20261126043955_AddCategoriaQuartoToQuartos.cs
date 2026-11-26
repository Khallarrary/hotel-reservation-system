using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaQuartoToQuartos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Quartos");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Quartos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CategoriaQuartoId",
                table: "Quartos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CategoriaQuartos_HotelId_Id",
                table: "CategoriaQuartos",
                columns: new[] { "HotelId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Quartos_HotelId_CategoriaQuartoId",
                table: "Quartos",
                columns: new[] { "HotelId", "CategoriaQuartoId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Quartos_CategoriaQuartos_HotelId_CategoriaQuartoId",
                table: "Quartos",
                columns: new[] { "HotelId", "CategoriaQuartoId" },
                principalTable: "CategoriaQuartos",
                principalColumns: new[] { "HotelId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quartos_CategoriaQuartos_HotelId_CategoriaQuartoId",
                table: "Quartos");

            migrationBuilder.DropIndex(
                name: "IX_Quartos_HotelId_CategoriaQuartoId",
                table: "Quartos");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CategoriaQuartos_HotelId_Id",
                table: "CategoriaQuartos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Quartos");

            migrationBuilder.DropColumn(
                name: "CategoriaQuartoId",
                table: "Quartos");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Quartos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
