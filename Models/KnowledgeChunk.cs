using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBot.Models
{
    public class KnowledgeChunk : BaseEntity
    {
        public KnowledgeSource Source { get; set; }

        // ArticleId or DocumentId
        public Guid SourceId { get; set; }

        public string Content { get; set; }

        // Vector embedding
        [NotMapped] // Replace later if using pgvector
        public float[] Embedding { get; set; }
    }
}
