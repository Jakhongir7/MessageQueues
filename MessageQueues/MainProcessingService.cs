using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageQueues
{
    public class MainProcessingService
    {
        private readonly IModel _channel;
        private readonly string _outputFolder;

        public MainProcessingService(IModel channel, string outputFolder)
        {
            _channel = channel;
            _outputFolder = outputFolder;
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, eventArguments) =>
            {
                var body = eventArguments.Body.ToArray();
                var fileName = $"{Guid.NewGuid()}.pdf"; // Generate unique file name for now
                var filePath = Path.Combine(_outputFolder, fileName);
                File.WriteAllBytes(filePath, body);
                Console.WriteLine($"[x] Received '{fileName}' from Data Capture Service");
            };

            _channel.BasicConsume(queue: Program.QueueName,
                                  autoAck: true,
                                  consumer: consumer);

            Console.WriteLine("Main Processing Service is running.");
        }
    }
}