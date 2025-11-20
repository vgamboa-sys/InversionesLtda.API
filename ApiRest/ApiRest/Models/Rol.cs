namespace ApiRest.Models
{
    public class Rol
    {
        public int Id { get; set; }                 // PK
        public string Nombre { get; set; }          // "ADMIN", "CLIENTE", etc.

        public ICollection<UsuarioRol> UsuarioRoles { get; set; }
    }
}
