using ApiRest.Context;
using ApiRest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinciaController : ControllerBase
    {


        private readonly AppDbContext _context;

        public ProvinciaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Provincia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvinciaDto>>> GetProvincias()
        {
            var provincias = await _context.Provincia
                .Select(p => new ProvinciaDto
                {
                    CodRegion = p.CodRegion,
                    CodProvincia = p.CodProvincia,
                    NombreProvincia = p.NombreProvincia
                })
                .ToListAsync();

            return Ok(provincias);
        }

        // POST: api/Provincia
        [HttpPost]
        public async Task<ActionResult<ProvinciaDto>> CreateProvincia(ProvinciaDto provinciaDto)
        {
            if (await _context.Provincia.AnyAsync(p =>
                p.CodRegion == provinciaDto.CodRegion && p.CodProvincia == provinciaDto.CodProvincia))
            {
                return BadRequest("La provincia con este ID ya existe.");
            }

            var provincia = new Provincia
            {
                CodRegion = provinciaDto.CodRegion,
                CodProvincia = provinciaDto.CodProvincia,
                NombreProvincia = provinciaDto.NombreProvincia
            };

            _context.Provincia.Add(provincia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProvincias), new { id = provincia.CodProvincia }, provinciaDto);
        }










    }
}
