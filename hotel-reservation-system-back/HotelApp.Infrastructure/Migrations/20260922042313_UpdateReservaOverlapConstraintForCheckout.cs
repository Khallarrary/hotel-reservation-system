using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReservaOverlapConstraintForCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
               """
                ALTER TABLE "Reservas"
                DROP CONSTRAINT IF EXISTS "ImpedirConcorrencia";
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "Reservas"
                ADD CONSTRAINT "ImpedirConcorrencia"
                EXCLUDE USING gist
                (
                    "HotelId" WITH =,
                    "QuartoId" WITH =,
                    tstzrange("CheckIn", "CheckOut", '[)') WITH &&
                )
                WHERE ("Status" IN (0, 1));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Reservas"
                DROP CONSTRAINT IF EXISTS "ImpedirConcorrencia";
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "Reservas"
                ADD CONSTRAINT "ImpedirConcorrencia"
                EXCLUDE USING gist
                (
                    "HotelId" WITH =,
                    "QuartoId" WITH =,
                    tstzrange("CheckIn", "CheckOut", '[)') WITH &&
                )
                WHERE ("Status" <> 3);
                """);
        }
    }
}
