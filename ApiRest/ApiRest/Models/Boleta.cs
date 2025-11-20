namespace ApiRest.Models
{
    public class Boleta
    {
        public int CodBoleta { get; set; }          // PK

        public DateTime Fecha { get; set; }

        // Relación con Cliente
        public int RunCliente { get; set; }         // FK a Cliente.NumRun
        public Cliente Cliente { get; set; }

        // Info de contacto de ESTA compra
        public string CorreoContacto { get; set; }  // correo al que se envió la boleta
        public bool EsInvitada { get; set; }        // true si la compra fue "como invitado"

        // Relación con Transacción/Tarjeta (Transbank)
        public string CodTransaccion { get; set; }  // FK a Tarjeta.CodTransaccion
        public Tarjeta Tarjeta { get; set; }

        public int Total { get; set; }              // total en CLP

        // Navegación a detalles
        public ICollection<DetalleBoleta> Detalles { get; set; }
    }
}
