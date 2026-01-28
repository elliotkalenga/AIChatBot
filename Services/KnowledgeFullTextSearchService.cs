using ChatBot.Data;
using ChatBot.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Services
{
    public class KnowledgeFullTextSearchService
    {
        private readonly ApplicationDbContext _db;

        public KnowledgeFullTextSearchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<KnowledgeChunk>> SearchAsync(string question, int limit = 5)
        {
            // Sanitize input for CONTAINS
            var terms = question
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 3)
                .Select(w => $"\"{w}*\"");

            if (!terms.Any())
                return new List<KnowledgeChunk>();

            var searchCondition = string.Join(" AND ", terms);

            var sql = $@"
                SELECT TOP ({limit}) kc.*
                FROM KnowledgeChunks kc
                INNER JOIN CONTAINSTABLE(
                    KnowledgeChunks,
                    Content,
                    @search
                ) ft ON kc.Id = ft.[KEY]
                ORDER BY ft.RANK DESC";

            return await _db.KnowledgeChunks
                .FromSqlRaw(sql,
                    new SqlParameter("@search", searchCondition))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
