namespace ApiRest.Models
{
    public class Stock
    {
        public int CodStock { get; set; } // Clave primaria manual

        // Relación con Producto
        public int CodProducto { get; set; }
        public Producto Producto { get; set; }

        // Relación con Sucursal
        public int CodSucursal { get; set; }
        public Sucursal Sucursal { get; set; }

        public int Cantidad { get; set; } // Cantidad de productos disponibles en la sucursal
    }
}
