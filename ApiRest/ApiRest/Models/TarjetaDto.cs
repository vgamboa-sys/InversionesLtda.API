namespace ApiRest.Models
{
    public class TarjetaDto
    {
        public string CodTransaccion { get; set; } // Clave primaria(numero tarjeta)
        public int NumTarjeta { get; set; } // Clave primaria(numero tarjeta)
        public string NombreTransaccion { get; set; }
    }

    public class CreateTarjetaDto
    {
        public string CodTransaccion { get; set; } // Clave primaria(numero tarjeta)
        public int NumTarjeta { get; set; } // Clave primaria(numero tarjeta)
        public string NombreTransaccion { get; set; }

    }
}