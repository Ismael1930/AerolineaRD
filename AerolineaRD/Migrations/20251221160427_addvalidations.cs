using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class addvalidations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificacionesAeronave",
                table: "Tripulacion",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TiempoDescansoMinutos",
                table: "Tripulacion",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CapacidadVuelosPorHora",
                table: "Aeropuerto",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TiempoPreparacionMinutos",
                table: "Aeronave",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificacionesAeronave",
                table: "Tripulacion");

            migrationBuilder.DropColumn(
                name: "TiempoDescansoMinutos",
                table: "Tripulacion");

            migrationBuilder.DropColumn(
                name: "CapacidadVuelosPorHora",
                table: "Aeropuerto");

            migrationBuilder.DropColumn(
                name: "TiempoPreparacionMinutos",
                table: "Aeronave");
        }
    }
}
