using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageQueues
{
    public class MainProcessingService
    {
        private const string QueueName = "ResultsQueue";
        private static readonly string LocalFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProcessedFiles");

        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                if (!Directory.Exists(LocalFolder))
                {
                    Directory.CreateDirectory(LocalFolder);
                }

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    File.WriteAllBytes(Path.Combine(LocalFolder, $"{Guid.NewGuid()}.pdf"), body);
                };
                channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            }
        }
    }
}
