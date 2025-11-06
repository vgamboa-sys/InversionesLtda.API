using ApiRest.Context;
using ApiRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Stock
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetStocks()
        {
            var stocks = await _context.Stock
                .Include(st => st.Producto) // Cargar Producto relacionado
                .Include(st => st.Sucursal) // Cargar Sucursal relacionada
                .Select(st => new StockDto
                {
                    CodStock = st.CodStock,
                    CodProducto = st.CodProducto,
                    CodSucursal = st.CodSucursal,
                    Cantidad = st.Cantidad
                })
                .ToListAsync();

            return Ok(stocks);
        }

        // POST: api/Stock
        [HttpPost]
        public async Task<ActionResult<StockDto>> CreateStock(StockDto stockDto)
        {
            // Validar que el Producto asociado exista
            var producto = await _context.Producto.FindAsync(stockDto.CodProducto);
            if (producto == null)
            {
                return BadRequest("El producto asociado no existe.");
            }

            // Validar que la Sucursal asociada exista
            var sucursal = await _context.Sucursal.FindAsync(stockDto.CodSucursal);
            if (sucursal == null)
            {
                return BadRequest("La sucursal asociada no existe.");
            }

            var stock = new Stock
            {
                CodStock = stockDto.CodStock,
                CodProducto = stockDto.CodProducto,
                CodSucursal = stockDto.CodSucursal,
                Cantidad = stockDto.Cantidad
            };

            _context.Stock.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStocks), new { id = stock.CodStock }, stockDto);
        }
    }
}
