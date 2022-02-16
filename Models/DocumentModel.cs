using System.ComponentModel.DataAnnotations;

namespace Prospecto.Models
{
    public class DocumentModel
    {
        public int Id { get; set; }

        [MaxLength(250)]
        public string FileName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string FileType { get; set; } = string.Empty;

        [MaxLength]
        public byte[]? FileData { get; set; }
        public DateTime Created { get; set; }
        
        public DateTime Modified { get; set; }
        public int IdProspecto { get; set; }

    }
}
