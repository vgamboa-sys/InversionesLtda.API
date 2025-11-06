namespace ApiRest.Models
{
    public class Region
    {
        public int CodRegion { get; set; } // Clave primaria
        public string NombreRegion { get; set; }

        // Relación con Provincia
        public ICollection<Provincia> Provincia { get; set; }

        public ICollection<Cliente> Cliente { get; set; }

    }
}
