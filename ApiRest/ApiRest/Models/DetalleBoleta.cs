namespace ApiRest.Models
{
    public class DetalleBoleta
    {
        public int Id { get; set; }                 // PK identity

        // Relación con Boleta
        public int CodBoleta { get; set; }
        public Boleta Boleta { get; set; }

        // Relación con Producto
        public int CodProducto { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }     // Precio en CLP al momento de la venta

        // Si quieres, puedes agregar un campo calculado total de línea:
        // public int TotalLinea => Cantidad * PrecioUnitario;
    }
}
