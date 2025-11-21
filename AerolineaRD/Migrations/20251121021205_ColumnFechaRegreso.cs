using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class ColumnFechaRegreso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegreso",
                table: "Vuelo",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegreso",
                table: "Vuelo");
        }
    }
}
