using ApiRest.Context;
using ApiRest.Dto.Request;
using ApiRest.Dto.Response;
using ApiRest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoletaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BoletaController(AppDbContext context)
        {
            _context = context;
        }

        // Helper: mapear entidad Boleta -> BoletaDto
        private static BoletaDto MapToDto(Boleta b, List<DetalleBoleta> detalles, List<Producto> productos)
        {
            var detalleDtos = detalles
                .Select(d =>
                {
                    var prod = productos.First(p => p.CodProducto == d.CodProducto);
                    return new DetalleBoletaDto
                    {
                        CodProducto = d.CodProducto,
                        NombreProducto = prod.NombreProducto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        TotalLinea = d.Cantidad * d.PrecioUnitario
                    };
                })
                .ToList();

            return new BoletaDto
            {
                CodBoleta = b.CodBoleta,
                Fecha = b.Fecha,
                RunCliente = b.RunCliente,
                CorreoContacto = b.CorreoContacto,
                EsInvitada = b.EsInvitada,
                CodTransaccion = b.CodTransaccion,
                Total = b.Total,
                Detalles = detalleDtos
            };
        }

        // POST: api/Boleta
        [HttpPost]
        public async Task<ActionResult<BoletaDto>> CrearBoleta([FromBody] BoletaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Detalles == null || !request.Detalles.Any())
                return BadRequest(new { mensaje = "La boleta debe contener al menos un producto." });

            // 1) Validar transacción de pago (Tarjeta) ya registrada por TransbankController
            var tarjeta = await _context.Tarjeta
                .FirstOrDefaultAsync(t => t.CodTransaccion == request.CodTransaccion);

            if (tarjeta == null)
            {
                return BadRequest(new { mensaje = "La transacción de pago no existe o no está registrada." });
            }

            // 2) Validar cliente existente (asumimos que se crea antes vía ClienteController)
            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(c => c.NumRun == request.NumRun);

            if (cliente == null)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente no existe. Regístralo primero antes de emitir la boleta."
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 3) Validar productos y stock, y calcular total
                int total = 0;
                var codProductos = request.Detalles.Select(d => d.CodProducto).Distinct().ToList();

                var productos = await _context.Producto
                    .Where(p => codProductos.Contains(p.CodProducto))
                    .ToListAsync();

                if (productos.Count != codProductos.Count)
                {
                    var faltantes = codProductos.Except(productos.Select(p => p.CodProducto)).ToList();
                    return BadRequest(new { mensaje = $"Los siguientes productos no existen: {string.Join(", ", faltantes)}" });
                }

                // Cargar stock de la sucursal indicada
                var stocks = await _context.Stock
                    .Where(s => s.CodSucursal == request.CodSucursal && codProductos.Contains(s.CodProducto))
                    .ToListAsync();

                foreach (var item in request.Detalles)
                {
                    var producto = productos.First(p => p.CodProducto == item.CodProducto);

                    var stock = stocks.FirstOrDefault(s => s.CodProducto == item.CodProducto);
                    if (stock == null || stock.Cantidad < item.Cantidad)
                    {
                        return BadRequest(new
                        {
                            mensaje = $"No hay stock suficiente para el producto {producto.NombreProducto} en la sucursal {request.CodSucursal}."
                        });
                    }

                    total += producto.Precio * item.Cantidad;
                }

                // 4) Crear Boleta
                var boleta = new Boleta
                {
                    Fecha = DateTime.Now,
                    RunCliente = cliente.NumRun,
                    CodTransaccion = request.CodTransaccion,
                    Total = total,
                    CorreoContacto = request.CorreoContacto,
                    EsInvitada = request.EsInvitada
                };

                _context.Boleta.Add(boleta);
                await _context.SaveChangesAsync(); // para obtener CodBoleta

                // 5) Crear detalles y descontar stock
                var detallesBoleta = new List<DetalleBoleta>();

                foreach (var item in request.Detalles)
                {
                    var producto = productos.First(p => p.CodProducto == item.CodProducto);
                    var stock = stocks.First(s => s.CodProducto == item.CodProducto);

                    var detalle = new DetalleBoleta
                    {
                        CodBoleta = boleta.CodBoleta,
                        CodProducto = item.CodProducto,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio
                    };

                    detallesBoleta.Add(detalle);
                    _context.DetalleBoleta.Add(detalle);

                    // Descontar stock
                    stock.Cantidad -= item.Cantidad;
                    _context.Stock.Update(stock);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 6) Mapear a DTO y devolver
                var boletaDto = MapToDto(boleta, detallesBoleta, productos);
                return Ok(boletaDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error interno al crear la boleta.", error = ex.Message });
            }
        }

        // GET: api/Boleta/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BoletaDto>> ObtenerBoleta(int id)
        {
            var boleta = await _context.Boleta
                .FirstOrDefaultAsync(b => b.CodBoleta == id);

            if (boleta == null)
                return NotFound(new { mensaje = "Boleta no encontrada." });

            var detalles = await _context.DetalleBoleta
                .Where(d => d.CodBoleta == boleta.CodBoleta)
                .ToListAsync();

            var codProductos = detalles.Select(d => d.CodProducto).Distinct().ToList();
            var productos = await _context.Producto
                .Where(p => codProductos.Contains(p.CodProducto))
                .ToListAsync();

            var dto = MapToDto(boleta, detalles, productos);
            return Ok(dto);
        }

        // GET: api/Boleta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoletaDto>>> ListarBoletas()
        {
            var boletas = await _context.Boleta
                .OrderByDescending(b => b.Fecha)
                .Take(100) // por ejemplo, últimas 100
                .ToListAsync();

            var codBoletas = boletas.Select(b => b.CodBoleta).ToList();

            var detalles = await _context.DetalleBoleta
                .Where(d => codBoletas.Contains(d.CodBoleta))
                .ToListAsync();

            var codProductos = detalles.Select(d => d.CodProducto).Distinct().ToList();

            var productos = await _context.Producto
                .Where(p => codProductos.Contains(p.CodProducto))
                .ToListAsync();

            var lista = new List<BoletaDto>();

            foreach (var b in boletas)
            {
                var detBoleta = detalles.Where(d => d.CodBoleta == b.CodBoleta).ToList();
                var dto = MapToDto(b, detBoleta, productos);
                lista.Add(dto);
            }

            return Ok(lista);
        }
    }
}
