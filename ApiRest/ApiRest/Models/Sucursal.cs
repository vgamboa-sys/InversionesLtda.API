namespace ApiRest.Models
{
    public class Sucursal
    {
        public int CodSucursal { get; set; } // Clave primaria manual
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        // Relación con Comuna
        public int CodRegion { get; set; }
        public int CodProvincia { get; set; }
        public int CodComuna { get; set; }
        public Comuna Comuna { get; set; }

        // Relación con Stock
        public ICollection<Stock> Stock { get; set; } // Una sucursal puede tener varios stocks
    }
}
