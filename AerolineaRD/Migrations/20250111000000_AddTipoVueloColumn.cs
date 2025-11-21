using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
  public partial class AddTipoVueloColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    migrationBuilder.AddColumn<string>(
     name: "TipoVuelo",
   table: "Vuelo",
     type: "TEXT",
     maxLength: 15,
          nullable: true,
             defaultValue: "SoloIda");
      }

  /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
       migrationBuilder.DropColumn(
     name: "TipoVuelo",
   table: "Vuelo");
    }
    }
}
