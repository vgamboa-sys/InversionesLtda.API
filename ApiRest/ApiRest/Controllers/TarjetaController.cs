using ApiRest.Context;
using ApiRest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class TarjetaController : ControllerBase
{
    private readonly AppDbContext _context;

    public TarjetaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Tarjeta
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TarjetaDto>>> GetTarjetas()
    {
        var tarjetas = await _context.Tarjeta
            .Select(t => new TarjetaDto
            {
                CodTransaccion = t.CodTransaccion,
                NumTarjeta = t.NumTarjeta,
                NombreTransaccion = t.NombreTransaccion
            })
            .ToListAsync();

        return Ok(tarjetas);
    }

    // POST: api/Tarjeta
    [HttpPost]
    public async Task<ActionResult<TarjetaDto>> PostTarjeta(CreateTarjetaDto tarjetaDto)
    {
        var tarjeta = new Tarjeta
        {
            CodTransaccion = tarjetaDto.CodTransaccion,
            NumTarjeta = tarjetaDto.NumTarjeta,
            NombreTransaccion = tarjetaDto.NombreTransaccion
        };

        _context.Tarjeta.Add(tarjeta);
        await _context.SaveChangesAsync();

        var tarjetaResponse = new TarjetaDto
        {
            CodTransaccion = tarjeta.CodTransaccion,
            NombreTransaccion = tarjeta.NombreTransaccion
        };

        return CreatedAtAction(nameof(GetTarjetas), new { id = tarjeta.CodTransaccion }, tarjetaResponse);
    }
}
