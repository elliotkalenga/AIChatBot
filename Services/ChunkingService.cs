namespace ChatBot.Services
{
    public class ChunkingService : IChunkingService
    {
        public List<string> Chunk(string text, int chunkSize = 800)
        {
            var chunks = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
                return chunks;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i += chunkSize)
            {
                chunks.Add(string.Join(" ",
                    words.Skip(i).Take(chunkSize)));
            }

            return chunks;
        }
    }
}
