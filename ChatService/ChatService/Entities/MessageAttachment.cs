namespace ChatService.Entities
{
    public class MessageAttachment
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string FileUrl { get; set; } = string.Empty; // MinIO, S3, Cloudinary...
        public string Type { get; set; } = "image"; // image | audio | video | file
    }

}
