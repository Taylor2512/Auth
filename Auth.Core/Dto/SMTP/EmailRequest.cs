 

namespace Auth.Core.Dto.SMTP
{
    public record EmailRequest
    {
        public required string To { get; set; }
        public List<string>? CC { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; }= string.Empty;
        public List< AttachmentRequest>? Attachment { get; set; }

    }
}
