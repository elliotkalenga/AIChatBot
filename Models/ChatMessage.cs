namespace ChatBot.Models
{
    public class ChatMessage : BaseEntity
    {
        public string SessionId { get; set; }

        public string UserMessage { get; set; }

        public string BotResponse { get; set; }
    }
}
