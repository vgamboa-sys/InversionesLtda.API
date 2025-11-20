namespace ApiRest.Models
{
    public class Cliente
    {
        public int NumRun { get; set; }             // PK (RUN numérico)
        public string DvRun { get; set; }           // DV (k o número)

        public string P_Nombre { get; set; }
        public string S_Nombre { get; set; }
        public string A_Paterno { get; set; }
        public string A_Materno { get; set; }

        public string Correo { get; set; }          // correo "principal"
        public string Direccion { get; set; }

        // Territorio
        public int CodRegion { get; set; }
        public Region Region { get; set; }

        public int CodProvincia { get; set; }
        public Provincia Provincia { get; set; }

        public int CodComuna { get; set; }
        public Comuna Comuna { get; set; }

        // Relación opcional con Usuario (null = compró como invitado / sin cuenta)
        public int? UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Navegación a Boletas
        public ICollection<Boleta> Boletas { get; set; }
    }
}
