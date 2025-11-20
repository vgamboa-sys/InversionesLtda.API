namespace ApiRest.Dto.Request
{
    public class BoletaRequest
    {
        // Cliente
        public int NumRun { get; set; }
        public string DvRun { get; set; }

        public string CorreoContacto { get; set; }
        public bool EsInvitada { get; set; }

        // Sucursal desde donde sale el stock
        public int CodSucursal { get; set; }

        // Transacción de pago ya confirmada por Transbank
        public string CodTransaccion { get; set; }

        // Detalle de la venta
        public List<DetalleVentaDto> Detalles { get; set; }
    }

    public class DetalleVentaDto
    {
        public int CodProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
