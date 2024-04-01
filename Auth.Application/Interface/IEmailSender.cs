

using Auth.Core.Dto.SMTP;

namespace Auth.Application.Interface
{
    public interface IEmailSender
    {
        Task EnqueueEmailAsync(EmailRequest emailRequest);
    }
}
