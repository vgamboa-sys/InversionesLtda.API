namespace ApiRest.Models
{
    public class Cliente
    {
        public int NumRun { get; set; } // Clave primaria manual (Run del cliente)
        public string DvRun { get; set; } // Dígito verificador
        public string P_Nombre { get; set; } // Primer nombre
        public string S_Nombre { get; set; } // Segundo nombre
        public string A_Paterno { get; set; } // Apellido paterno
        public string A_Materno { get; set; } // Apellido materno
        public string Correo { get; set; } // Correo electrónico
        public string Direccion { get; set; } // Dirección

        // Relación con Region
        public int CodRegion { get; set; } // FK con Region
        public Region Region { get; set; } // Relación uno a muchos

        // Relación con Provincia
        public int CodProvincia { get; set; } // FK con Provincia
        public Provincia Provincia { get; set; } // Relación uno a muchos

        // Relación con Comuna
        public int CodComuna { get; set; } // FK con Comuna
        public Comuna Comuna { get; set; } // Relación uno a muchos
        public ICollection<Boleta> Boleta { get; set; }
    }
}
