namespace ApiRest.Dto.Response
{
    public class BoletaDto
    {
        public int CodBoleta { get; set; }
        public DateTime Fecha { get; set; }

        public int RunCliente { get; set; }
        public string CorreoContacto { get; set; }
        public bool EsInvitada { get; set; }

        public string CodTransaccion { get; set; }
        public int Total { get; set; }

        public List<DetalleBoletaDto> Detalles { get; set; }
    }

    public class DetalleBoletaDto
    {
        public int CodProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
        public int TotalLinea { get; set; }
    }
}
