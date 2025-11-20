using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiRest.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
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
                name: "Stock");

            migrationBuilder.DropTable(
                name: "Tarjeta");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropTable(
                name: "Comuna");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
