using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class addMoreValidations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Clase",
                table: "Vuelo",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clase",
                table: "Vuelo");
        }
    }
}
