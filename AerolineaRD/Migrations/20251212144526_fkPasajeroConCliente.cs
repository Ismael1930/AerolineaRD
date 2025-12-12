using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class fkPasajeroConCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "Pasajero",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pasajero_IdCliente",
                table: "Pasajero",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Pasajero_Cliente_IdCliente",
                table: "Pasajero",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "IdCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pasajero_Cliente_IdCliente",
                table: "Pasajero");

            migrationBuilder.DropIndex(
                name: "IX_Pasajero_IdCliente",
                table: "Pasajero");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Pasajero");
        }
    }
}
