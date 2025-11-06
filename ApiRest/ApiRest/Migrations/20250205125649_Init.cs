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
                name: "Tarjeta",
                columns: table => new
                {
                    CodTransaccion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumTarjeta = table.Column<int>(type: "int", nullable: false),
                    NombreTransaccion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjeta", x => x.CodTransaccion);
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
                name: "Boleta",
                columns: table => new
                {
                    CodBoleta = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodTransaccion = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false),
                    RunCliente = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boleta", x => x.CodBoleta);
                    table.ForeignKey(
                        name: "FK_Boleta_Tarjeta_CodTransaccion",
                        column: x => x.CodTransaccion,
                        principalTable: "Tarjeta",
                        principalColumn: "CodTransaccion",
                        onDelete: ReferentialAction.Restrict);
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
                name: "DetalleBoleta",
                columns: table => new
                {
                    CodDetalle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodBoleta = table.Column<int>(type: "int", nullable: false),
                    CodProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleBoleta", x => x.CodDetalle);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    NumRun = table.Column<int>(type: "int", nullable: false),
                    DvRun = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    P_Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    S_Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    A_Paterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    A_Materno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodRegion = table.Column<int>(type: "int", nullable: false),
                    CodProvincia = table.Column<int>(type: "int", nullable: false),
                    CodComuna = table.Column<int>(type: "int", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
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
                name: "BoletaCliente",
                columns: table => new
                {
                    BoletaCodBoleta = table.Column<int>(type: "int", nullable: false),
                    ClienteNumRun = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoletaCliente", x => new { x.BoletaCodBoleta, x.ClienteNumRun });
                    table.ForeignKey(
                        name: "FK_BoletaCliente_Boleta_BoletaCodBoleta",
                        column: x => x.BoletaCodBoleta,
                        principalTable: "Boleta",
                        principalColumn: "CodBoleta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoletaCliente_Cliente_ClienteNumRun",
                        column: x => x.ClienteNumRun,
                        principalTable: "Cliente",
                        principalColumn: "NumRun",
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

            migrationBuilder.CreateIndex(
                name: "IX_Boleta_CodTransaccion",
                table: "Boleta",
                column: "CodTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_BoletaCliente_ClienteNumRun",
                table: "BoletaCliente",
                column: "ClienteNumRun");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_CodRegion_CodProvincia_CodComuna",
                table: "Cliente",
                columns: new[] { "CodRegion", "CodProvincia", "CodComuna" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoletaCliente");

            migrationBuilder.DropTable(
                name: "DetalleBoleta");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Boleta");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropTable(
                name: "Tarjeta");

            migrationBuilder.DropTable(
                name: "Comuna");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
