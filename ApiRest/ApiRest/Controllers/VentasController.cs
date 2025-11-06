using ApiRest.Context;
using ApiRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("RealizarVenta")]
        public async Task<IActionResult> RealizarVenta([FromBody] VentaDto ventaDto)
        {
            // Validación inicial
            if (ventaDto == null || ventaDto.DetalleProductos == null || !ventaDto.DetalleProductos.Any())
            {
                return BadRequest("Datos de venta incompletos.");
            }

            // Calcular el total de la venta
            int totalVenta = ventaDto.DetalleProductos.Sum(dp => dp.Cantidad * dp.PrecioUnitario);

            // Crear la Boleta
            var boleta = new Boleta
            {
                CodBoleta = ventaDto.CodBoleta,
                Fecha = DateTime.Now,
                CodTransaccion = ventaDto.CodTransaccion,
                RunCliente = ventaDto.RunCliente, // Asignar el RunCliente
                Total = totalVenta // Asignar el total calculado
            };
            _context.Boleta.Add(boleta);

            // Agregar los Detalles de Boleta
            foreach (var detalle in ventaDto.DetalleProductos)
            {
                // Validar stock disponible
                var productoStock = await _context.Stock
                    .FirstOrDefaultAsync(s => s.CodProducto == detalle.CodProducto && s.CodSucursal == detalle.CodSucursal);

                if (productoStock == null || productoStock.Cantidad < detalle.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para el producto con ID {detalle.CodProducto}.");
                }

                // Actualizar Stock
                productoStock.Cantidad -= detalle.Cantidad;

                // Crear el DetalleBoleta
                var detalleBoleta = new DetalleBoleta
                {
                    CodDetalle = Guid.NewGuid().ToString("N"), // Generar ID único
                    CodBoleta = boleta.CodBoleta,
                    CodProducto = detalle.CodProducto,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario
                };
                _context.DetalleBoleta.Add(detalleBoleta);
            }

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Venta realizada con éxito.", Total = totalVenta });
        }
    }
}
