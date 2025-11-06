namespace ApiRest.Models
{
    public class ClienteDto
    {
        public int NumRun { get; set; }
        public string DvRun { get; set; }
        public string P_Nombre { get; set; }
        public string S_Nombre { get; set; }
        public string A_Paterno { get; set; }
        public string A_Materno { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }

        // Relación con Region, Provincia y Comuna
        public int CodRegion { get; set; }
        public int CodProvincia { get; set; }
        public int CodComuna { get; set; }
    }
}
