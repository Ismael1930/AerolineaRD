using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class SistemaTripulacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipo",
                columns: table => new
                {
                    IdEquipo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimoVueloFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisponibleDesde = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipo", x => x.IdEquipo);
                });

            migrationBuilder.CreateTable(
                name: "Personal",
                columns: table => new
                {
                    IdPersonal = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Licencia = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CertificacionesAeronave = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    TiempoDescansoMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    UltimoVueloFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaContratacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal", x => x.IdPersonal);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionEquipoAeronave",
                columns: table => new
                {
                    IdAsignacion = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdEquipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Matricula = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaDesasignacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionEquipoAeronave", x => x.IdAsignacion);
                    table.ForeignKey(
                        name: "FK_AsignacionEquipoAeronave_Aeronave_Matricula",
                        column: x => x.Matricula,
                        principalTable: "Aeronave",
                        principalColumn: "Matricula");
                    table.ForeignKey(
                        name: "FK_AsignacionEquipoAeronave_Equipo_IdEquipo",
                        column: x => x.IdEquipo,
                        principalTable: "Equipo",
                        principalColumn: "IdEquipo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipoPersonal",
                columns: table => new
                {
                    IdEquipoPersonal = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdEquipo = table.Column<int>(type: "INTEGER", nullable: false),
                    IdPersonal = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipoPersonal", x => x.IdEquipoPersonal);
                    table.ForeignKey(
                        name: "FK_EquipoPersonal_Equipo_IdEquipo",
                        column: x => x.IdEquipo,
                        principalTable: "Equipo",
                        principalColumn: "IdEquipo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipoPersonal_Personal_IdPersonal",
                        column: x => x.IdPersonal,
                        principalTable: "Personal",
                        principalColumn: "IdPersonal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionEquipoAeronave_IdEquipo",
                table: "AsignacionEquipoAeronave",
                column: "IdEquipo");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionEquipoAeronave_Matricula_Activa",
                table: "AsignacionEquipoAeronave",
                columns: new[] { "Matricula", "Activa" },
                unique: true,
                filter: "[Activa] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EquipoPersonal_IdEquipo",
                table: "EquipoPersonal",
                column: "IdEquipo");

            migrationBuilder.CreateIndex(
                name: "IX_EquipoPersonal_IdPersonal",
                table: "EquipoPersonal",
                column: "IdPersonal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionEquipoAeronave");

            migrationBuilder.DropTable(
                name: "EquipoPersonal");

            migrationBuilder.DropTable(
                name: "Equipo");

            migrationBuilder.DropTable(
                name: "Personal");
        }
    }
}
