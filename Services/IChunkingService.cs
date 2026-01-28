namespace ChatBot.Services
{
    public interface IChunkingService
    {
        List<string> Chunk(string text, int chunkSize = 800);
    }
}
