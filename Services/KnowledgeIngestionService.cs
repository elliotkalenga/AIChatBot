using ChatBot.Data;
using ChatBot.Models;

namespace ChatBot.Services
{
    public class KnowledgeIngestionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IDocumentParserService _parser;
        private readonly IChunkingService _chunker;

        public KnowledgeIngestionService(
            ApplicationDbContext db,
            IDocumentParserService parser,
            IChunkingService chunker)
        {
            _db = db;
            _parser = parser;
            _chunker = chunker;
        }

        public async Task IngestDocumentAsync(KnowledgeDocument doc)
        {
            var text = _parser.Parse(doc.StoredFilePath);
            var chunks = _chunker.Chunk(text);

            foreach (var chunk in chunks)
            {
                _db.KnowledgeChunks.Add(new KnowledgeChunk
                {
                    Source = doc.SourceType,
                    SourceId = doc.Id,
                    Content = chunk
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task IngestArticleAsync(KnowledgeArticle article)
        {
            var chunks = _chunker.Chunk(article.Content);

            foreach (var chunk in chunks)
            {
                _db.KnowledgeChunks.Add(new KnowledgeChunk
                {
                    Source = KnowledgeSource.DatabaseText,
                    SourceId = article.Id,
                    Content = chunk
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}
