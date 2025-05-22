namespace ChatService.Entities
{
    public class MessageEmoji
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string Emoji { get; set; } = "😄";
    }

}
