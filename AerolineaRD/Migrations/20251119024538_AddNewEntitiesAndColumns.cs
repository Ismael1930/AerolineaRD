using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntitiesAndColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duracion",
                table: "Vuelo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Vuelo",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraLlegada",
                table: "Vuelo",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraSalida",
                table: "Vuelo",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Matricula",
                table: "Vuelo",
                type: "TEXT",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioBase",
                table: "Vuelo",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Reserva",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumAsiento",
                table: "Reserva",
                type: "TEXT",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioTotal",
                table: "Reserva",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "EstadoPago",
                table: "Factura",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEmision",
                table: "Factura",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Aeronave",
                columns: table => new
                {
                    Matricula = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Capacidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aeronave", x => x.Matricula);
                });

            migrationBuilder.CreateTable(
                name: "EstadoVuelo",
                columns: table => new
                {
                    IdEstado = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdVuelo = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    HoraSalida = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HoraLlegada = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HoraSalidaProgramada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoraLlegadaProgramada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Puerta = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoVuelo", x => x.IdEstado);
                    table.ForeignKey(
                        name: "FK_EstadoVuelo_Vuelo_IdVuelo",
                        column: x => x.IdVuelo,
                        principalTable: "Vuelo",
                        principalColumn: "IdVuelo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificacion",
                columns: table => new
                {
                    IdNotificacion = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Leida = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacion", x => x.IdNotificacion);
                    table.ForeignKey(
                        name: "FK_Notificacion_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketSoporte",
                columns: table => new
                {
                    IdTicket = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    Asunto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Prioridad = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSoporte", x => x.IdTicket);
                    table.ForeignKey(
                        name: "FK_TicketSoporte_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tripulacion",
                columns: table => new
                {
                    IdTripulacion = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Licencia = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tripulacion", x => x.IdTripulacion);
                });

            migrationBuilder.CreateTable(
                name: "VueloTripulacion",
                columns: table => new
                {
                    IdVueloTripulacion = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdVuelo = table.Column<int>(type: "INTEGER", nullable: false),
                    IdTripulacion = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VueloTripulacion", x => x.IdVueloTripulacion);
                    table.ForeignKey(
                        name: "FK_VueloTripulacion_Tripulacion_IdTripulacion",
                        column: x => x.IdTripulacion,
                        principalTable: "Tripulacion",
                        principalColumn: "IdTripulacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VueloTripulacion_Vuelo_IdVuelo",
                        column: x => x.IdVuelo,
                        principalTable: "Vuelo",
                        principalColumn: "IdVuelo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vuelo_Matricula",
                table: "Vuelo",
                column: "Matricula");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_NumAsiento",
                table: "Reserva",
                column: "NumAsiento");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoVuelo_IdVuelo",
                table: "EstadoVuelo",
                column: "IdVuelo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificacion_IdCliente",
                table: "Notificacion",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSoporte_IdCliente",
                table: "TicketSoporte",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_VueloTripulacion_IdTripulacion",
                table: "VueloTripulacion",
                column: "IdTripulacion");

            migrationBuilder.CreateIndex(
                name: "IX_VueloTripulacion_IdVuelo",
                table: "VueloTripulacion",
                column: "IdVuelo");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Asiento_NumAsiento",
                table: "Reserva",
                column: "NumAsiento",
                principalTable: "Asiento",
                principalColumn: "NumAsiento");

            migrationBuilder.AddForeignKey(
                name: "FK_Vuelo_Aeronave_Matricula",
                table: "Vuelo",
                column: "Matricula",
                principalTable: "Aeronave",
                principalColumn: "Matricula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Asiento_NumAsiento",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Vuelo_Aeronave_Matricula",
                table: "Vuelo");

            migrationBuilder.DropTable(
                name: "Aeronave");

            migrationBuilder.DropTable(
                name: "EstadoVuelo");

            migrationBuilder.DropTable(
                name: "Notificacion");

            migrationBuilder.DropTable(
                name: "TicketSoporte");

            migrationBuilder.DropTable(
                name: "VueloTripulacion");

            migrationBuilder.DropTable(
                name: "Tripulacion");

            migrationBuilder.DropIndex(
                name: "IX_Vuelo_Matricula",
                table: "Vuelo");

            migrationBuilder.DropIndex(
                name: "IX_Reserva_NumAsiento",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "HoraLlegada",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "HoraSalida",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "Matricula",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "PrecioBase",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "NumAsiento",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "PrecioTotal",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "EstadoPago",
                table: "Factura");

            migrationBuilder.DropColumn(
                name: "FechaEmision",
                table: "Factura");
        }
    }
}
