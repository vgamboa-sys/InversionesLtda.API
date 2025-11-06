namespace ApiRest.Models
{
    public class Provincia
    {
        public int CodRegion { get; set; } // Clave compuesta (parte de la relación con Region)
        public int CodProvincia { get; set; } // Clave primaria

        public string NombreProvincia { get; set; }

        // Relación con Region
        public Region Region { get; set; }

        // Relación con Comuna
        public ICollection<Comuna> Comuna { get; set; }

        public ICollection<Cliente> Cliente { get; set; }


    }
}
