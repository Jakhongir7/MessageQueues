using RabbitMQ.Client;

namespace MessageQueues
{
    public class Program
    {
        public const string QueueName = "MessageQueue";
        //UPDATE PATH BEFORE RUN
        public const string FolderPath = @"C:\Users\Jakhongir_Bakhodirov\Desktop\New folder";
        public const string OutputFolderPath = @"C:\Users\Jakhongir_Bakhodirov\Desktop\New";

        static void Main(string[] args)
        {
            // Create connection to RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            // Declare queue for communication
            channel.QueueDeclare(queue: QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Initialize and start Data Capture Service
            var dataCaptureService = new DataCaptureService(channel, FolderPath);
            dataCaptureService.Start();

            // Initialize and start Main Processing Service
            var mainProcessingService = new MainProcessingService(channel, OutputFolderPath);
            mainProcessingService.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine(); // Wait for user input to exit
        }
    }
}