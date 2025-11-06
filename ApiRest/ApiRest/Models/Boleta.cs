namespace ApiRest.Models
{
    public class Boleta
    {
        public int CodBoleta { get; set; } // Clave primaria
        public DateTime Fecha { get; set; } // Fecha de la transacción
        public string CodTransaccion { get; set; } // FK hacia Tarjeta (opcional)
        public Tarjeta Tarjeta { get; set; } // Relación con Tarjeta
        public ICollection<DetalleBoleta> DetalleBoleta { get; set; } // Relación con DetalleBoleta
        public ICollection<Cliente> Cliente { get; set; } // Relación con Cliente

        public int Total { get; set; } // Nuevo campo Total
        public string RunCliente { get; set; } // Nuevo campo Run del cliente
    }
}
