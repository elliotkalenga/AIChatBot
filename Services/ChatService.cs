namespace ChatBot.Services
{
    public class ChatService
    {
        private readonly LocalSearchService _search;

        public ChatService(LocalSearchService search)
        {
            _search = search;
        }

        public async Task<string> AnswerAsync(string question)
        {
            var chunks = await _search.SearchAsync(question);

            if (!chunks.Any())
                return "I don't have that information yet.";

            return string.Join("\n\n", chunks.Select(c => c.Content));
        }
    }
}
