using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConFunT.Models
{
    public class RoleMaster
    {
        [Key]
        [Display(Name = "Id")]
        public int? ID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre Del Rol")]
        public string Rol { get; set; }=String.Empty;
 
    }
}
