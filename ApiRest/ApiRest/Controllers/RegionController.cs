using ApiRest.Context;
using ApiRest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {

        private readonly AppDbContext _context;

        public RegionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Region
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegions()
        {
            var regions = await _context.Region
                .Select(r => new RegionDto
                {
                    CodRegion = r.CodRegion,
                    NombreRegion = r.NombreRegion
                })
                .ToListAsync();

            return Ok(regions);
        }

        // POST: api/Region
        [HttpPost]
        public async Task<ActionResult<RegionDto>> CreateRegion(RegionDto regionDto)
        {
            if (await _context.Region.AnyAsync(r => r.CodRegion == regionDto.CodRegion))
            {
                return BadRequest("La región con este ID ya existe.");
            }

            var region = new Region
            {
                CodRegion = regionDto.CodRegion,
                NombreRegion = regionDto.NombreRegion
            };

            _context.Region.Add(region);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegions), new { id = region.CodRegion }, regionDto);
        }

    }
}
