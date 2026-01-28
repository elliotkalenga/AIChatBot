using System.ComponentModel.DataAnnotations;

namespace ChatBot.Models
{
    public class KnowledgeDocument : BaseEntity
    {
        [Required]
        public string? FileName { get; set; }

        [Required]
        public string ? StoredFilePath { get; set; }

        [Required]
        public KnowledgeSource SourceType { get; set; }

        [MaxLength(100)]
        public string? UploadedBy { get; set; }


    }
}
