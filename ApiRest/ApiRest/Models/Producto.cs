namespace ApiRest.Models
{
    public class Producto
    {
        public int CodProducto { get; set; } // Clave primaria manual
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public int Precio { get; set; }

        // Relación con Stock
        public ICollection<Stock> Stock { get; set; } // Un producto puede estar en varios stocks
        
    }
}
