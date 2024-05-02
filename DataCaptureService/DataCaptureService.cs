using RabbitMQ.Client;

namespace MessageQueues
{
    public class DataCaptureService
    {
        private const string QueueName = "ResultsQueue";
        private const string MonitorFolder = "ScannedFiles";

        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                if (!Directory.Exists(MonitorFolder))
                {
                    Directory.CreateDirectory(MonitorFolder);
                }

                var fileWatcher = new FileSystemWatcher(MonitorFolder);
                fileWatcher.Filter = "*.pdf";
                fileWatcher.Created += (sender, e) =>
                {
                    var filePath = e.FullPath;
                    var fileBytes = File.ReadAllBytes(filePath);
                    channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: fileBytes);
                };
                fileWatcher.EnableRaisingEvents = true;
                Console.WriteLine("Data Capture Service started. Monitoring folder for PDF files...");
                Console.ReadLine(); // Keep the service running
            }
        }
    }
}