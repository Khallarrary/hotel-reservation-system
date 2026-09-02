using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservaOverlapConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

      
            migrationBuilder.Sql
                ("""
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                    ALTER TABLE "Reservas"
                    DROP CONSTRAINT IF EXISTS "ImpedirConcorrencia";
                """);
        }
    }
}
