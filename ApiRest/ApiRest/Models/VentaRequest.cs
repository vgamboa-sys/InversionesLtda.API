namespace ApiRest.Models
{
    public class VentaRequest
    {
        public int CodBoleta { get; set; }
        public string CodTransaccion { get; set; } // Clave primaria(numero tarjeta)
        public int ClienteNumRun { get; set; } // RUN del cliente
        public List<DetalleProductoRequest> DetalleProductos { get; set; }
    }


    public class DetalleProductoRequest
    {
        public int CodProducto { get; set; }
        public int CodSucursal { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
    }

}   

