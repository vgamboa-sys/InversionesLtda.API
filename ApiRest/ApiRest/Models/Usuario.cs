namespace ApiRest.Models
{
    public class Usuario
    {
        public int Id { get; set; }                 // PK identidad

        public string Username { get; set; }        // único, para login
        public string PasswordHash { get; set; }    // hash, nunca password plano
        public string Email { get; set; }
        public bool Activo { get; set; } = true;

        // Relación 1–N con UsuarioRol
        public ICollection<UsuarioRol> UsuarioRoles { get; set; }

        // Relación 1–1 opcional con Cliente (si este usuario es un cliente del negocio)
        public Cliente Cliente { get; set; }
    }
}
