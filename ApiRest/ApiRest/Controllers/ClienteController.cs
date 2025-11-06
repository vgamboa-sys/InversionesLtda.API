using ApiRest.Context;
using ApiRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Cliente
                .Select(cl => new ClienteDto
                {
                    NumRun = cl.NumRun,
                    DvRun = cl.DvRun,
                    P_Nombre = cl.P_Nombre,
                    S_Nombre = cl.S_Nombre,
                    A_Paterno = cl.A_Paterno,
                    A_Materno = cl.A_Materno,
                    Correo = cl.Correo,
                    Direccion = cl.Direccion,
                    CodRegion = cl.CodRegion,
                    CodProvincia = cl.CodProvincia,
                    CodComuna = cl.CodComuna
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // GET: api/Cliente/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetClienteById(int id)
        {
            var cliente = await _context.Cliente
                .Where(cl => cl.NumRun == id)
                .Select(cl => new ClienteDto
                {
                    NumRun = cl.NumRun,
                    DvRun = cl.DvRun,
                    P_Nombre = cl.P_Nombre,
                    S_Nombre = cl.S_Nombre,
                    A_Paterno = cl.A_Paterno,
                    A_Materno = cl.A_Materno,
                    Correo = cl.Correo,
                    Direccion = cl.Direccion,
                    CodRegion = cl.CodRegion,
                    CodProvincia = cl.CodProvincia,
                    CodComuna = cl.CodComuna
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // POST: api/Cliente
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CreateCliente([FromBody] ClienteDto clienteDto)
        {
            // Validar que la región, provincia y comuna existan
            var regionExists = await _context.Region.AnyAsync(r => r.CodRegion == clienteDto.CodRegion);
            if (!regionExists)
                return BadRequest("La región asociada no existe.");

            var provinciaExists = await _context.Provincia.AnyAsync(p =>
                p.CodRegion == clienteDto.CodRegion && p.CodProvincia == clienteDto.CodProvincia);
            if (!provinciaExists)
                return BadRequest("La provincia asociada no existe.");

            var comunaExists = await _context.Comuna.AnyAsync(c =>
                c.CodRegion == clienteDto.CodRegion &&
                c.CodProvincia == clienteDto.CodProvincia &&
                c.CodComuna == clienteDto.CodComuna);
            if (!comunaExists)
                return BadRequest("La comuna asociada no existe.");

            var cliente = new Cliente
            {
                NumRun = clienteDto.NumRun,
                DvRun = clienteDto.DvRun,
                P_Nombre = clienteDto.P_Nombre,
                S_Nombre = clienteDto.S_Nombre,
                A_Paterno = clienteDto.A_Paterno,
                A_Materno = clienteDto.A_Materno,
                Correo = clienteDto.Correo,
                Direccion = clienteDto.Direccion,
                CodRegion = clienteDto.CodRegion,
                CodProvincia = clienteDto.CodProvincia,
                CodComuna = clienteDto.CodComuna
            };

            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClienteById), new { id = cliente.NumRun }, clienteDto);
        }

        // PUT: api/Cliente/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteDto clienteDto)
        {
            if (id != clienteDto.NumRun)
            {
                return BadRequest("El ID del cliente no coincide.");
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            // Actualizar los datos del cliente
            cliente.DvRun = clienteDto.DvRun;
            cliente.P_Nombre = clienteDto.P_Nombre;
            cliente.S_Nombre = clienteDto.S_Nombre;
            cliente.A_Paterno = clienteDto.A_Paterno;
            cliente.A_Materno = clienteDto.A_Materno;
            cliente.Correo = clienteDto.Correo;
            cliente.Direccion = clienteDto.Direccion;
            cliente.CodRegion = clienteDto.CodRegion;
            cliente.CodProvincia = clienteDto.CodProvincia;
            cliente.CodComuna = clienteDto.CodComuna;

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Cliente/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(cl => cl.NumRun == id);
        }
    }
}
