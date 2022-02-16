using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Prospecto.Models
{
    public class CalleModel
    {
        [Key]
        [Display(Name = "Id")]
        public int ID { get; set; } 

        [DataType(DataType.Text)]
        [Display(Name = "Colonia")]
        [UIHint("List")]
        public List<SelectListItem> Col { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Colonia")]
        public string CalleCol { get; set; } = string.Empty;
        public string CodigoCol { get; set; } = string.Empty;
        public string NomCol { get; private set; } = string.Empty;

        [Required]
        [Display(Name = "Calle")]
        [StringLength(5)]
        public string CodigoCalle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(80, ErrorMessage = "Longitud Del Nombre No Puede Ser Mayor a 80 Caracteres")]
        public string Nombre { get; set; } = string.Empty;

        public string NomCalleNew { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;

        public CalleModel() //Constructor
        {
            Col = new List<SelectListItem>();
        }

    }
}
