using ChatBot.Data;
using ChatBot.Models;
using ChatBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly KnowledgeIngestionService _ingestion;

        public AdminController(
            ApplicationDbContext db,
            KnowledgeIngestionService ingestion)
        {
            _db = db;
            _ingestion = ingestion;
        }

        // =========================
        // LIST DOCUMENTS
        // =========================
        public IActionResult Index()
        {
            var documents = _db.KnowledgeDocuments
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            return View(documents);
        }

        // =========================
        // UPLOAD FORM
        // =========================
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // =========================
        // HANDLE UPLOAD
        // =========================
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file.");
                return View();
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var ext = Path.GetExtension(file.FileName).ToLower();
            var sourceType = ext switch
            {
                ".pdf" => KnowledgeSource.Pdf,
                ".docx" => KnowledgeSource.Word,
                ".xlsx" => KnowledgeSource.Excel,
                _ => KnowledgeSource.DatabaseText
            };

            var doc = new KnowledgeDocument
            {
                FileName = file.FileName,
                StoredFilePath = filePath,
                SourceType = sourceType,
                UploadedBy = User?.Identity?.Name
            };

            _db.KnowledgeDocuments.Add(doc);
            await _db.SaveChangesAsync();

            // ✅ Direct ingestion (simple & reliable)
            await _ingestion.IngestDocumentAsync(doc);

            TempData["Message"] = "Document uploaded and indexed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
