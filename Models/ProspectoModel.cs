using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Prospecto.Models
{
	public class ProspectoModel
	{
		[Key]
		[Display(Name = "Id")]
		public int ID { get; set; }

		[Required]
		[Display(Name = "Nombre"), StringLength(100)]
		public string Nombre { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Primer Apellido"), StringLength(80)]
		public string PrimerApellido { get; set; } = string.Empty;

		[Display(Name = "Segundo Apellido"),StringLength(80),MinLength(0)]
		public string? SegundoApellido { get; set; } = string.Empty;


		[DataType(DataType.Text)]
		[Display(Name = "Calle")]
		[UIHint("List")]
		public List<SelectListItem> Cal1 { get; set; } 
		[Required]
		[Display(Name = "Nombre De La Calle"), StringLength(5)]
		public string Calle { get; set; } = string.Empty;

		[Required]
		[Display(Name = "No Casa"), StringLength(10)]
		public string Numero { get; set; } = string.Empty;

		[Required]
		[StringLength(5)]
		public string Colonia { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Telefono"), StringLength(10)]
		public string Telefono { get; set; } = string.Empty;

		[Required]
		[Display(Name = "RFC"), StringLength(15)]
		public string RFC { get; set; } = string.Empty;

		[Display(Name = "Estatus"), StringLength(1), MinLength(0)]
		public string? Estatus { get; set; } = string.Empty;
		public int IdDoc { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Colonia (teclee el nombre)")]
        public string SCol1 { get; set; } = "";

		[DataType(DataType.Text)]
		[Display(Name = "CP")]
		public string CP { get; set; } = "";
		public string CalleNom { get; set; } = "";
		public string ColoniaNom { get; set; } = "";
		public string Comentario { get; set; } = "";

		public ProspectoModel()
        {
			Cal1 = new List<SelectListItem>();
		}

    }
}
