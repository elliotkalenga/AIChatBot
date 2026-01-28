using System.ComponentModel.DataAnnotations;

namespace ChatBot.Models
{
    public class KnowledgeArticle : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }
    }
}
