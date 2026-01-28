using ChatBot.Data;
using ChatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Services
{
    public class KnowledgeSearchService
    {
        private readonly ApplicationDbContext _db;

        public KnowledgeSearchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<KnowledgeChunk>> SearchAsync(string question, int limit = 5)
        {
            // 1️⃣ Tokenize question
            var keywords = question
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 3) // ignore short words
                .Distinct()
                .ToList();

            if (!keywords.Any())
                return new List<KnowledgeChunk>();

            // 2️⃣ Load chunks (small KB → OK)
            var chunks = await _db.KnowledgeChunks.ToListAsync();

            // 3️⃣ Score chunks by keyword matches
            var ranked = chunks
                .Select(chunk => new
                {
                    Chunk = chunk,
                    Score = keywords.Count(k =>
                        chunk.Content.ToLower().Contains(k))
                })
                .Where(x => x.Score > 0) // must match at least one keyword
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Chunk.CreatedAt)
                .Take(limit)
                .Select(x => x.Chunk)
                .ToList();

            return ranked;
        }
    }
}
