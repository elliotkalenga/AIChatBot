using System.ComponentModel.DataAnnotations;

namespace ChatBot.ViewModels
{
    public class ChatRequestVm
    {
        [Required]
        public string Question { get; set; }
    }
}
