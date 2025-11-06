namespace ApiRest.Models
{
    public class SucursalDto
    {
        public int CodSucursal { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        // Relación con Comuna
        public int CodRegion { get; set; }
        public int CodProvincia { get; set; }
        public int CodComuna { get; set; }
    }
}
