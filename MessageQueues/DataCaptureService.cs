using RabbitMQ.Client;

namespace MessageQueues
{
    public class DataCaptureService
    {
        private readonly IModel _channel;
        private readonly string _folderPath;

        public DataCaptureService(IModel channel, string folderPath)
        {
            _channel = channel;
            _folderPath = folderPath;
        }

        public void Start()
        {
            Console.WriteLine("Data Capture Service is running.");

            var watcher = new FileSystemWatcher();
            watcher.Path = _folderPath;
            watcher.Filter = "*.pdf"; // Assuming we are starting with PDF files

            // Process existing files
            foreach (var existingFile in Directory.GetFiles(_folderPath, "*.pdf"))
            {
                ProcessDocument(existingFile);
            }

            // Set up event handler to process new files
            watcher.Created += (sender, e) => ProcessDocument(e.FullPath);
            watcher.EnableRaisingEvents = true;
        }

        private void ProcessDocument(string fullPath)
        {
            var fileName = Path.GetFileName(fullPath);
            // Add a delay to ensure the file is completely written to disk
            Thread.Sleep(1000); // Delay for 1 second (adjust as needed)
            var body = File.ReadAllBytes(fullPath);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(exchange: "",
                                  routingKey: Program.QueueName,
                                  basicProperties: properties,
                                  body: body);

            Console.WriteLine($"[x] Sent '{fileName}' to Main Processing Service");
        }
    }
}