using System.Collections.Generic;

namespace ApiRest.Dto.Response
{
    public class LoginResponseDto
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }
        public List<string> Roles { get; set; }
    }
}
