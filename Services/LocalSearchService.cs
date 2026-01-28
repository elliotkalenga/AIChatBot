using ChatBot.Data;
using ChatBot.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Services
{
    public class LocalSearchService
    {
        private readonly ApplicationDbContext _db;

        public LocalSearchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<KnowledgeChunk>> SearchAsync(string query, int top = 5)
        {
            var sql = @"
                SELECT TOP (@top) KC.*
                FROM KnowledgeChunks KC
                INNER JOIN CONTAINSTABLE(KnowledgeChunks, Content, @q) FT
                    ON KC.Id = FT.[KEY]
                ORDER BY FT.RANK DESC";

            return await _db.KnowledgeChunks
                .FromSqlRaw(sql,
                    new SqlParameter("@top", top),
                    new SqlParameter("@q", query))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
