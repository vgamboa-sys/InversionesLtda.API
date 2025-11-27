using ApiRest.Context;
using ApiRest.Dto.Request;
using ApiRest.Dto.Response;
using ApiRest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ApiRest.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Método interno para hashear la contraseña (SHA256)
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Login de usuario (Admin o Usuario normal).
        /// Basado en tablas Usuario, Rol, UsuarioRol.
        /// NO afecta la lógica de Cliente invitado.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.UsernameOrEmail) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Credenciales inválidas."
                });
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u =>
                    u.Username == request.UsernameOrEmail ||
                    u.Email == request.UsernameOrEmail);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    exito = false,
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            if (!usuario.Activo)
            {
                return Unauthorized(new
                {
                    exito = false,
                    mensaje = "El usuario se encuentra inactivo."
                });
            }

            var hashInput = HashPassword(request.Password);
            if (usuario.PasswordHash != hashInput)
            {
                return Unauthorized(new
                {
                    exito = false,
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            var roles = await _context.UsuarioRol
                .Where(ur => ur.UsuarioId == usuario.Id)
                .Include(ur => ur.Rol)
                .Select(ur => ur.Rol.Nombre)
                .ToListAsync();

            var data = new LoginResponseDto
            {
                UsuarioId = usuario.Id,
                Username = usuario.Username,
                Email = usuario.Email,
                Activo = usuario.Activo,
                Roles = roles
            };

            return Ok(new
            {
                exito = true,
                mensaje = "Login correcto.",
                data
            });
        }

        /// <summary>
        /// Registro básico de usuario (rol "Usuario").
        /// No crea Cliente todavía (lo podemos hacer después vía Flask).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Datos de registro incompletos."
                });
            }

            var existeUsername = await _context.Usuario.AnyAsync(u => u.Username == request.Username);
            if (existeUsername)
            {
                return Conflict(new
                {
                    exito = false,
                    mensaje = "El nombre de usuario ya existe."
                });
            }

            var existeEmail = await _context.Usuario.AnyAsync(u => u.Email == request.Email);
            if (existeEmail)
            {
                return Conflict(new
                {
                    exito = false,
                    mensaje = "El correo ya está registrado."
                });
            }

            var usuario = new Usuario
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Activo = true
            };

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // Buscar rol "Usuario"
            var rolUsuario = await _context.Rol.FirstOrDefaultAsync(r => r.Nombre == "Usuario");
            if (rolUsuario != null)
            {
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId = usuario.Id,
                    RolId = rolUsuario.Id
                };
                _context.UsuarioRol.Add(usuarioRol);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                exito = true,
                mensaje = "Usuario registrado correctamente.",
                data = new
                {
                    usuario.Id,
                    usuario.Username,
                    usuario.Email
                }
            });
        }

        /// <summary>
        /// Reset de contraseña simple (sin correo).
        /// Busca por username o email y actualiza PasswordHash.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.UsernameOrEmail) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new
                {
                    exito = false,
                    mensaje = "Datos inválidos."
                });
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u =>
                    u.Username == request.UsernameOrEmail ||
                    u.Email == request.UsernameOrEmail);

            if (usuario == null)
            {
                return NotFound(new
                {
                    exito = false,
                    mensaje = "Usuario no encontrado."
                });
            }

            usuario.PasswordHash = HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                exito = true,
                mensaje = "Contraseña actualizada correctamente."
            });
        }
    }
}
