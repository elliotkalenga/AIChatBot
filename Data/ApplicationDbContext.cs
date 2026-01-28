using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChatBot.Models;

namespace ChatBot.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===== Knowledge Base =====
        public DbSet<KnowledgeArticle> KnowledgeArticles { get; set; }
        public DbSet<KnowledgeDocument> KnowledgeDocuments { get; set; }
        public DbSet<KnowledgeChunk> KnowledgeChunks { get; set; }

        // ===== Chat History =====
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional indexes for performance
            builder.Entity<KnowledgeChunk>()
                .HasIndex(x => x.Source);

            builder.Entity<KnowledgeArticle>()
                .HasIndex(x => x.Category);
        }
    }
}
