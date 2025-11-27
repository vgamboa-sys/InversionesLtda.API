using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiRest.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    CodProducto = table.Column<int>(type: "int", nullable: false),
                    NombreProducto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.CodProducto);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    CodRegion = table.Column<int>(type: "int", nullable: false),
                    NombreRegion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.CodRegion);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tarjeta",
                columns: table => new
                {
                    CodTransaccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumTarjeta = table.Column<int>(type: "int", nullable: false),
                    NombreTransaccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjeta", x => x.CodTransaccion);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    CodRegion = table.Column<int>(type: "int", nullable: false),
                    CodProvincia = table.Column<int>(type: "int", nullable: false),
                    NombreProvincia = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => new { x.CodRegion, x.CodProvincia });
                    table.ForeignKey(
                        name: "FK_Provincia_Region_CodRegion",
                        column: x => x.CodRegion,
                        principalTable: "Region",
                        principalColumn: "CodRegion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comuna",
                columns: table => new
                {
                    CodRegion = table.Column<int>(type: "int", nullable: false),
                    CodProvincia = table.Column<int>(type: "int", nullable: false),
                    CodComuna = table.Column<int>(type: "int", nullable: false),
                    NombreComuna = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comuna", x => new { x.CodRegion, x.CodProvincia, x.CodComuna });
                    table.ForeignKey(
                        name: "FK_Comuna_Provincia_CodRegion_CodProvincia",
                        columns: x => new { x.CodRegion, x.CodProvincia },
                        principalTable: "Provincia",
                        principalColumns: new[] { "CodRegion", "CodProvincia" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    NumRun = table.Column<int>(type: "int", nullable: false),
                    DvRun = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    P_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    S_Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    A_Paterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    A_Materno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CodRegion = table.Column<int>(type: "int", nullable: false),
                    CodProvincia = table.Column<int>(type: "int", nullable: false),
                    CodComuna = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.NumRun);
                    table.ForeignKey(
                        name: "FK_Cliente_Comuna_CodRegion_CodProvincia_CodComuna",
                        columns: x => new { x.CodRegion, x.CodProvincia, x.CodComuna },
                        principalTable: "Comuna",
                        principalColumns: new[] { "CodRegion", "CodProvincia", "CodComuna" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cliente_Provincia_CodRegion_CodProvincia",
                        columns: x => new { x.CodRegion, x.CodProvincia },
                        principalTable: "Provincia",
                        principalColumns: new[] { "CodRegion", "CodProvincia" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cliente_Region_CodRegion",
                        column: x => x.CodRegion,
                        principalTable: "Region",
                        principalColumn: "CodRegion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cliente_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Sucursal",
                columns: table => new
                {
                    CodSucursal = table.Column<int>(type: "int", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodRegion = table.Column<int>(type: "int", nullable: false),
                    CodProvincia = table.Column<int>(type: "int", nullable: false),
                    CodComuna = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursal", x => x.CodSucursal);
                    table.ForeignKey(
                        name: "FK_Sucursal_Comuna_CodRegion_CodProvincia_CodComuna",
                        columns: x => new { x.CodRegion, x.CodProvincia, x.CodComuna },
                        principalTable: "Comuna",
                        principalColumns: new[] { "CodRegion", "CodProvincia", "CodComuna" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Boleta",
                columns: table => new
                {
                    CodBoleta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RunCliente = table.Column<int>(type: "int", nullable: false),
                    CorreoContacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EsInvitada = table.Column<bool>(type: "bit", nullable: false),
                    CodTransaccion = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boleta", x => x.CodBoleta);
                    table.ForeignKey(
                        name: "FK_Boleta_Cliente_RunCliente",
                        column: x => x.RunCliente,
                        principalTable: "Cliente",
                        principalColumn: "NumRun",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Boleta_Tarjeta_CodTransaccion",
                        column: x => x.CodTransaccion,
                        principalTable: "Tarjeta",
                        principalColumn: "CodTransaccion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    CodStock = table.Column<int>(type: "int", nullable: false),
                    CodProducto = table.Column<int>(type: "int", nullable: false),
                    CodSucursal = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.CodStock);
                    table.ForeignKey(
                        name: "FK_Stock_Producto_CodProducto",
                        column: x => x.CodProducto,
                        principalTable: "Producto",
                        principalColumn: "CodProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stock_Sucursal_CodSucursal",
                        column: x => x.CodSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "CodSucursal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleBoleta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodBoleta = table.Column<int>(type: "int", nullable: false),
                    CodProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleBoleta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleBoleta_Boleta_CodBoleta",
                        column: x => x.CodBoleta,
                        principalTable: "Boleta",
                        principalColumn: "CodBoleta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleBoleta_Producto_CodProducto",
                        column: x => x.CodProducto,
                        principalTable: "Producto",
                        principalColumn: "CodProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boleta_CodTransaccion",
                table: "Boleta",
                column: "CodTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Boleta_RunCliente",
                table: "Boleta",
                column: "RunCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_CodRegion_CodProvincia_CodComuna",
                table: "Cliente",
                columns: new[] { "CodRegion", "CodProvincia", "CodComuna" });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente",
                column: "UsuarioId",
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleBoleta_CodBoleta",
                table: "DetalleBoleta",
                column: "CodBoleta");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleBoleta_CodProducto",
                table: "DetalleBoleta",
                column: "CodProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_CodProducto",
                table: "Stock",
                column: "CodProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_CodSucursal",
                table: "Stock",
                column: "CodSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursal_CodRegion_CodProvincia_CodComuna",
                table: "Sucursal",
                columns: new[] { "CodRegion", "CodProvincia", "CodComuna" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_RolId",
                table: "UsuarioRol",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleBoleta");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "Boleta");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Tarjeta");

            migrationBuilder.DropTable(
                name: "Comuna");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
