namespace ApiRest.Models
{
    public class VentaDto
    {
        public int CodBoleta { get; set; }
        public string CodTransaccion { get; set; } // Clave primaria(numero tarjeta)
        public string RunCliente { get; set; } // Nuevo campo
        public List<DetalleVentaDto> DetalleProductos { get; set; }
    }
}
