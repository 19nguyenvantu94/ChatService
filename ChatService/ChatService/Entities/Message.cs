namespace ChatService.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

        public List<MessageAttachment> Attachments { get; set; } = new();
        public List<MessageEmoji> Emojis { get; set; } = new();
    }
}
