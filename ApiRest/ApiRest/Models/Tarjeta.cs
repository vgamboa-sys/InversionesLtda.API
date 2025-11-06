namespace ApiRest.Models
{
    public class Tarjeta
    {
        public string CodTransaccion { get; set; } // Clave primaria(Buy Order)
        public int NumTarjeta { get; set; } // Clave primaria(numero tarjeta)
        public string NombreTransaccion { get; set; } // Tipo de transacción (e.g., compra online, en tienda)
        public ICollection<Boleta> Boleta { get; set; } // Relación con Boleta
    }
}
