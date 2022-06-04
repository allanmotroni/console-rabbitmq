using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace ConsoleRabbitMQPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            string queueName = "fila-1";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                int count = 1;
                while (true)
                {                    
                    string message = $"[{count++}] - Testing RabbitMQ Message";

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        basicProperties: null,
                        body: body);

                    Console.WriteLine($" [x] Message sent: {message}");

                    Thread.Sleep(250);
                }
            }
            
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
