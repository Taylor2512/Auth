namespace Auth.Core.Dto.SMTP
{
    public class AttachmentRequest
    {
        public required string FileName { get; set; }
        public required byte[] File { get; set; }
        public  string? ContentType { get; set; }
    }
}
