using ChatBot.Data;
using ChatBot.Models;
using ChatBot.Services;
using Microsoft.AspNetCore.Mvc;

public class KnowledgeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly KnowledgeIngestionService _ingestion;

    public KnowledgeController(ApplicationDbContext db,
        KnowledgeIngestionService ingestion)
    {
        _db = db;
        _ingestion = ingestion;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var path = Path.Combine("uploads", file.FileName);

        using var fs = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(fs);

        var doc = new KnowledgeDocument
        {
            FileName = file.FileName,
            StoredFilePath = path,
            SourceType = GetType(file.FileName)
        };

        _db.KnowledgeDocuments.Add(doc);
        await _db.SaveChangesAsync();

        await _ingestion.IngestDocumentAsync(doc);

        return Ok("Indexed successfully");
    }

    private KnowledgeSource GetType(string name) =>
        Path.GetExtension(name).ToLower() switch
        {
            ".pdf" => KnowledgeSource.Pdf,
            ".docx" => KnowledgeSource.Word,
            ".xlsx" => KnowledgeSource.Excel,
            _ => KnowledgeSource.DatabaseText
        };
}
