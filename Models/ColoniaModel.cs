using System.ComponentModel.DataAnnotations;

namespace Prospecto.Models
{
    public class ColoniaModel
    {
		[Key]
		[Display(Name = "Id")]
		public int ID { get; set; }

		[Display(Name = "Estado"), StringLength(3)]
		public string CveEdo { get; set; } = string.Empty;

		[Display(Name = "Municipio"), StringLength(3)]
		public string CveMun { get; set; } = string.Empty;

		[Display(Name = "Colonia"), StringLength(5)]
		public string CodigoCol { get; set; } = string.Empty;

		[Display(Name = "Colonia"), StringLength(150)]
		public string Nombre { get; set; } = string.Empty;

		[Display(Name = "CP"), StringLength(8)]
		public string CP { get; set; } = string.Empty;

		[Display(Name = "Tipo"), StringLength(50)]
		public string Tipo { get; set; } = string.Empty;

		[Display(Name = "Estatus"), StringLength(1)]
		public string Estatus { get; set; } = string.Empty;
	}
}
