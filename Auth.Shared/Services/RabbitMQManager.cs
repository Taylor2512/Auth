using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Text;

namespace Auth.Shared.Services
{
    public interface IRabbitMQManager
    {
        void DeclareQueue(string queueName, string exchangeName);
        void SendMessage(string queueName, string message);
        void ConsumeMessages(string queueName, Action<string> messageHandler);
    }

    public class RabbitMQManager : IRabbitMQManager, IDisposable
    {
        private bool disposed = false;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQManager(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQ");
            var hostName = rabbitMQConfig["HostName"];
            var userName = rabbitMQConfig["UserName"];
            var password = rabbitMQConfig["Password"];

            var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void DeclareQueue(string queueName, string exchangeName)
        {
            if (!_channel.IsOpen)
                throw new InvalidOperationException("RabbitMQ channel is not open.");

            // Declara el intercambio si no existe
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);

            // Declara la cola si no existe
            _channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Une la cola al intercambio
            _channel.QueueBind(queueName, exchangeName, routingKey: "");
        }

        public void SendMessage(string queueName, string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            DeclareQueue(queueName,$"exchange_{queueName}"); // Asegúrate de que la cola esté declarada antes de enviar el mensaje
            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent '{0}'", message);
        }

        public void ConsumeMessages(string queueName, Action<string> messageHandler)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messageHandler(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _channel.Dispose();
                _connection.Dispose();
            }

            disposed = true;
        }
    }
}
