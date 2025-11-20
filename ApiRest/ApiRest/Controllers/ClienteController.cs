using ApiRest.Context;
using ApiRest.Dto.Request;
using ApiRest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // Método helper para mapear entidad -> DTO
        private static ClienteDto MapToDto(Cliente c)
        {
            return new ClienteDto
            {
                NumRun = c.NumRun,
                DvRun = c.DvRun,
                P_Nombre = c.P_Nombre,
                S_Nombre = c.S_Nombre,
                A_Paterno = c.A_Paterno,
                A_Materno = c.A_Materno,
                Correo = c.Correo,
                Direccion = c.Direccion,
                CodRegion = c.CodRegion,
                CodProvincia = c.CodProvincia,
                CodComuna = c.CodComuna,
                UsuarioId = c.UsuarioId
            };
        }

        // GET: api/Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Cliente
                // Nota: aquí NO necesitamos Include, sólo usamos las columnas directas
                .ToListAsync();

            var dtos = clientes.Select(MapToDto).ToList();
            return Ok(dtos);
        }

        // GET: api/Cliente/12345678
        [HttpGet("{numRun:int}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int numRun)
        {
            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(c => c.NumRun == numRun);

            if (cliente == null)
            {
                return NotFound(new { mensaje = "Cliente no encontrado." });
            }

            var dto = MapToDto(cliente);
            return Ok(dto);
        }

        // POST: api/Cliente
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CrearCliente([FromBody] ClienteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verificar si el cliente ya existe por RUN
            var existe = await _context.Cliente.AnyAsync(c => c.NumRun == dto.NumRun);
            if (existe)
            {
                return Conflict(new { mensaje = "Ya existe un cliente con ese RUN." });
            }

            // Validar Región / Provincia / Comuna (igual que antes)
            var region = await _context.Region.FindAsync(dto.CodRegion);
            if (region == null)
                return BadRequest(new { mensaje = "La región especificada no existe." });

            var provincia = await _context.Provincia
                .FirstOrDefaultAsync(p => p.CodRegion == dto.CodRegion && p.CodProvincia == dto.CodProvincia);
            if (provincia == null)
                return BadRequest(new { mensaje = "La provincia especificada no existe." });

            var comuna = await _context.Comuna
                .FirstOrDefaultAsync(c => c.CodRegion == dto.CodRegion
                                        && c.CodProvincia == dto.CodProvincia
                                        && c.CodComuna == dto.CodComuna);
            if (comuna == null)
                return BadRequest(new { mensaje = "La comuna especificada no existe." });

            // Opcional: validar UsuarioId si viene
            if (dto.UsuarioId.HasValue)
            {
                var usuario = await _context.Usuario.FindAsync(dto.UsuarioId.Value);
                if (usuario == null)
                    return BadRequest(new { mensaje = "El usuario asociado no existe." });
            }

            var cliente = new Cliente
            {
                NumRun = dto.NumRun,
                DvRun = dto.DvRun,
                P_Nombre = dto.P_Nombre,
                S_Nombre = dto.S_Nombre,
                A_Paterno = dto.A_Paterno,
                A_Materno = dto.A_Materno,
                Correo = dto.Correo,
                Direccion = dto.Direccion,
                CodRegion = dto.CodRegion,
                CodProvincia = dto.CodProvincia,
                CodComuna = dto.CodComuna,
                UsuarioId = dto.UsuarioId
            };

            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            var creadoDto = MapToDto(cliente);

            // CreatedAtAction con DTO (no con la entidad)
            return CreatedAtAction(nameof(GetCliente), new { numRun = creadoDto.NumRun }, creadoDto);
        }

        // PUT: api/Cliente/12345678
        [HttpPut("{numRun:int}")]
        public async Task<ActionResult> ActualizarCliente(int numRun, [FromBody] ClienteDto dto)
        {
            if (numRun != dto.NumRun)
                return BadRequest(new { mensaje = "El RUN de la URL no coincide con el del cuerpo." });

            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.NumRun == numRun);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            // Validar territorios
            var region = await _context.Region.FindAsync(dto.CodRegion);
            if (region == null)
                return BadRequest(new { mensaje = "La región especificada no existe." });

            var provincia = await _context.Provincia
                .FirstOrDefaultAsync(p => p.CodRegion == dto.CodRegion && p.CodProvincia == dto.CodProvincia);
            if (provincia == null)
                return BadRequest(new { mensaje = "La provincia especificada no existe." });

            var comuna = await _context.Comuna
                .FirstOrDefaultAsync(c => c.CodRegion == dto.CodRegion
                                        && c.CodProvincia == dto.CodProvincia
                                        && c.CodComuna == dto.CodComuna);
            if (comuna == null)
                return BadRequest(new { mensaje = "La comuna especificada no existe." });

            // Usuario opcional
            if (dto.UsuarioId.HasValue)
            {
                var usuario = await _context.Usuario.FindAsync(dto.UsuarioId.Value);
                if (usuario == null)
                    return BadRequest(new { mensaje = "El usuario asociado no existe." });

                cliente.UsuarioId = dto.UsuarioId;
            }
            else
            {
                cliente.UsuarioId = null;
            }

            // Actualizar campos
            cliente.DvRun = dto.DvRun;
            cliente.P_Nombre = dto.P_Nombre;
            cliente.S_Nombre = dto.S_Nombre;
            cliente.A_Paterno = dto.A_Paterno;
            cliente.A_Materno = dto.A_Materno;
            cliente.Correo = dto.Correo;
            cliente.Direccion = dto.Direccion;
            cliente.CodRegion = dto.CodRegion;
            cliente.CodProvincia = dto.CodProvincia;
            cliente.CodComuna = dto.CodComuna;

            _context.Cliente.Update(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Cliente/12345678
        [HttpDelete("{numRun:int}")]
        public async Task<ActionResult> EliminarCliente(int numRun)
        {
            var cliente = await _context.Cliente
                .Include(c => c.Boletas)
                .FirstOrDefaultAsync(c => c.NumRun == numRun);

            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            if (cliente.Boletas != null && cliente.Boletas.Any())
                return BadRequest(new { mensaje = "No se puede eliminar un cliente con boletas asociadas." });

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
