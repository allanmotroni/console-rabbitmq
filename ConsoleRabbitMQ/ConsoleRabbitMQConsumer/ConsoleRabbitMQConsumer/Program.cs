using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsoleRabbitMQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostName = Environment.GetEnvironmentVariable("SERVER");
            var factory = new ConnectionFactory() { HostName = hostName ?? "rabbitmq" };

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

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        channel.BasicAck(ea.DeliveryTag, false);

                        Console.WriteLine($" [x] Message Received: {message}");
                    }
                    catch (Exception ex)
                    {                        
                        channel.BasicNack(ea.DeliveryTag, false, false);
                        Console.WriteLine($"Exception: {ex}");
                    }
                };

                channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);

                Console.WriteLine();
                Console.ReadKey();
            }
        }
    }
}
