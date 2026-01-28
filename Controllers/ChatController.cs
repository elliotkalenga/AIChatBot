using ChatBot.Data;
using ChatBot.Models;
using ChatBot.Services;
using ChatBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    public class ChatController : Controller
    {
        private readonly KnowledgeFullTextSearchService _searchService;
        private readonly ApplicationDbContext _db;

        public ChatController(
            KnowledgeFullTextSearchService searchService,
            ApplicationDbContext db)
        {
            _searchService = searchService;
            _db = db;
        }

        // =========================
        // CHAT UI
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // =========================
        // ASK QUESTION
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(ChatRequestVm model)
        {
            if (!ModelState.IsValid)
                return View("Index");

            // 1️⃣ Full-Text Search (TOP ranked chunks only)
            var results = await _searchService.SearchAsync(model.Question, limit: 5);

            // 2️⃣ Pick best answer
            var answer = results.Any()
                ? results.First().Content
                : "Sorry, I couldn't find an answer to your question.";

            // 3️⃣ Save chat history
            var chat = new ChatMessage
            {
                SessionId = HttpContext.Session.Id,
                UserMessage = model.Question,
                BotResponse = answer,
                CreatedAt = DateTime.UtcNow
            };

            _db.ChatMessages.Add(chat);
            await _db.SaveChangesAsync();

            // 4️⃣ Return response
            return View("Index", new ChatResponseVm
            {
                Question = model.Question,
                Answer = answer
            });
        }
    }
}
