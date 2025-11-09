using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaRD.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aeropuerto",
                columns: table => new
                {
                    CodAeropuerto = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Ciudad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Pais = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aeropuerto", x => x.CodAeropuerto);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pasajero",
                columns: table => new
                {
                    IdPasajero = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Pasaporte = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasajero", x => x.IdPasajero);
                });

            migrationBuilder.CreateTable(
                name: "Vuelo",
                columns: table => new
                {
                    IdVuelo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroVuelo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Origen = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Destino = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vuelo", x => x.IdVuelo);
                    table.ForeignKey(
                        name: "FK_Vuelo_Aeropuerto_Destino",
                        column: x => x.Destino,
                        principalTable: "Aeropuerto",
                        principalColumn: "CodAeropuerto");
                    table.ForeignKey(
                        name: "FK_Vuelo_Aeropuerto_Origen",
                        column: x => x.Origen,
                        principalTable: "Aeropuerto",
                        principalColumn: "CodAeropuerto");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.IdCliente);
                    table.ForeignKey(
                        name: "FK_Cliente_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Equipaje",
                columns: table => new
                {
                    NumEquipaje = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IdPasajero = table.Column<int>(type: "INTEGER", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipaje", x => x.NumEquipaje);
                    table.ForeignKey(
                        name: "FK_Equipaje_Pasajero_IdPasajero",
                        column: x => x.IdPasajero,
                        principalTable: "Pasajero",
                        principalColumn: "IdPasajero",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asiento",
                columns: table => new
                {
                    NumAsiento = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    IdVuelo = table.Column<int>(type: "INTEGER", nullable: false),
                    Clase = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Disponibilidad = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asiento", x => x.NumAsiento);
                    table.ForeignKey(
                        name: "FK_Asiento_Vuelo_IdVuelo",
                        column: x => x.IdVuelo,
                        principalTable: "Vuelo",
                        principalColumn: "IdVuelo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    CodReserva = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IdPasajero = table.Column<int>(type: "INTEGER", nullable: false),
                    IdVuelo = table.Column<int>(type: "INTEGER", nullable: false),
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaReserva = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.CodReserva);
                    table.ForeignKey(
                        name: "FK_Reserva_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_Pasajero_IdPasajero",
                        column: x => x.IdPasajero,
                        principalTable: "Pasajero",
                        principalColumn: "IdPasajero",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_Vuelo_IdVuelo",
                        column: x => x.IdVuelo,
                        principalTable: "Vuelo",
                        principalColumn: "IdVuelo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factura",
                columns: table => new
                {
                    CodFactura = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CodReserva = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factura", x => x.CodFactura);
                    table.ForeignKey(
                        name: "FK_Factura_Reserva_CodReserva",
                        column: x => x.CodReserva,
                        principalTable: "Reserva",
                        principalColumn: "CodReserva");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asiento_IdVuelo",
                table: "Asiento",
                column: "IdVuelo");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_UserId",
                table: "Cliente",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipaje_IdPasajero",
                table: "Equipaje",
                column: "IdPasajero");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_CodReserva",
                table: "Factura",
                column: "CodReserva",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdCliente",
                table: "Reserva",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdPasajero",
                table: "Reserva",
                column: "IdPasajero");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdVuelo",
                table: "Reserva",
                column: "IdVuelo");

            migrationBuilder.CreateIndex(
                name: "IX_Vuelo_Destino",
                table: "Vuelo",
                column: "Destino");

            migrationBuilder.CreateIndex(
                name: "IX_Vuelo_Origen",
                table: "Vuelo",
                column: "Origen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asiento");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Equipaje");

            migrationBuilder.DropTable(
                name: "Factura");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Pasajero");

            migrationBuilder.DropTable(
                name: "Vuelo");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Aeropuerto");
        }
    }
}
