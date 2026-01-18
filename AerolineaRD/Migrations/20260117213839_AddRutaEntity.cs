using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class AddRutaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ruta",
                columns: table => new
                {
                    IdRuta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrigenCodigo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DestinoCodigo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DuracionMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanciaKm = table.Column<int>(type: "INTEGER", nullable: true),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ruta", x => x.IdRuta);
                    table.ForeignKey(
                        name: "FK_Ruta_Aeropuerto_DestinoCodigo",
                        column: x => x.DestinoCodigo,
                        principalTable: "Aeropuerto",
                        principalColumn: "CodAeropuerto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ruta_Aeropuerto_OrigenCodigo",
                        column: x => x.OrigenCodigo,
                        principalTable: "Aeropuerto",
                        principalColumn: "CodAeropuerto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ruta_DestinoCodigo",
                table: "Ruta",
                column: "DestinoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Ruta_OrigenCodigo",
                table: "Ruta",
                column: "OrigenCodigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ruta");
        }
    }
}
