using Auth.Application.Interface;
using Auth.Core.Dto.SMTP;
using Auth.Shared.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Text.Json;



namespace Auth.Application.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IRabbitMQManager _rabbitMQManager;

        public EmailSender(IRabbitMQManager rabbitMQManager)
        {
            _rabbitMQManager = rabbitMQManager;
        }

        public  Task EnqueueEmailAsync(EmailRequest emailRequest)
        {
            try
            {
                var emailJson = JsonSerializer.Serialize(emailRequest);
                _rabbitMQManager.SendMessage("email_queue", emailJson);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encolando correo electrónico en RabbitMQ: {ex.Message}");
                throw new ArgumentException($"Error encolando correo electrónico en RabbitMQ: {ex.Message}");
            }
        }

    }
}
