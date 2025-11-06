using ApiRest.Context;
using ApiRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Producto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _context.Producto
                .Select(p => new ProductoDto
                {
                    CodProducto = p.CodProducto,
                    NombreProducto = p.NombreProducto,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio
                })
                .ToListAsync();

            return Ok(productos);
        }

        // POST: api/Producto
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> CreateProducto(ProductoDto productoDto)
        {
            var producto = new Producto
            {
                CodProducto = productoDto.CodProducto,
                NombreProducto = productoDto.NombreProducto,
                Descripcion = productoDto.Descripcion,
                Precio = productoDto.Precio
            };

            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductos), new { id = producto.CodProducto }, productoDto);
        }
    }
}
