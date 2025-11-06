namespace ApiRest.Models
{
    public class DetalleBoleta
    {
        public string CodDetalle { get; set; } // Cambiado a string
        public int CodBoleta { get; set; }
        public Boleta Boleta { get; set; }
        public int CodProducto { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
    }
}
