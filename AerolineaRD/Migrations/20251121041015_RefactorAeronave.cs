using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAeronave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asiento_Vuelo_IdVuelo",
                table: "Asiento");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Asiento_NumAsiento",
                table: "Reserva");

            migrationBuilder.DropIndex(
                name: "IX_Reserva_NumAsiento",
                table: "Reserva");

            migrationBuilder.DropIndex(
                name: "IX_Asiento_IdVuelo",
                table: "Asiento");

            migrationBuilder.DropColumn(
                name: "Disponibilidad",
                table: "Asiento");

            migrationBuilder.DropColumn(
                name: "IdVuelo",
                table: "Asiento");

            migrationBuilder.AddColumn<string>(
                name: "ClasesDisponibles",
                table: "Vuelo",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Clase",
                table: "Reserva",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Matricula",
                table: "Asiento",
                type: "TEXT",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroAsiento",
                table: "Asiento",
                type: "TEXT",
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asiento_Matricula",
                table: "Asiento",
                column: "Matricula");

            migrationBuilder.AddForeignKey(
                name: "FK_Asiento_Aeronave_Matricula",
                table: "Asiento",
                column: "Matricula",
                principalTable: "Aeronave",
                principalColumn: "Matricula");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asiento_Aeronave_Matricula",
                table: "Asiento");

            migrationBuilder.DropIndex(
                name: "IX_Asiento_Matricula",
                table: "Asiento");

            migrationBuilder.DropColumn(
                name: "ClasesDisponibles",
                table: "Vuelo");

            migrationBuilder.DropColumn(
                name: "Clase",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "Matricula",
                table: "Asiento");

            migrationBuilder.DropColumn(
                name: "NumeroAsiento",
                table: "Asiento");

            migrationBuilder.AddColumn<string>(
                name: "Disponibilidad",
                table: "Asiento",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdVuelo",
                table: "Asiento",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_NumAsiento",
                table: "Reserva",
                column: "NumAsiento");

            migrationBuilder.CreateIndex(
                name: "IX_Asiento_IdVuelo",
                table: "Asiento",
                column: "IdVuelo");

            migrationBuilder.AddForeignKey(
                name: "FK_Asiento_Vuelo_IdVuelo",
                table: "Asiento",
                column: "IdVuelo",
                principalTable: "Vuelo",
                principalColumn: "IdVuelo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Asiento_NumAsiento",
                table: "Reserva",
                column: "NumAsiento",
                principalTable: "Asiento",
                principalColumn: "NumAsiento");
        }
    }
}
