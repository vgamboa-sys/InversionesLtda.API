namespace ApiRest.Models
{
    public class Comuna
    {
        public int CodRegion { get; set; }
        public int CodProvincia { get; set; }
        public int CodComuna { get; set; }
        public string NombreComuna { get; set; }

        public Provincia Provincia { get; set; }

        public ICollection<Cliente> Cliente { get; set; }
        public ICollection<Sucursal> Sucursal { get; set; } // Relación con Sucursal

    }
}
