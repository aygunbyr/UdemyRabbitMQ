using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical=1,
        Error=2,
        Warning=3,
        Info=4,
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://uunkvvsy:whtUMIENsS2ct10W6LctKFwoTNqtKyfJ@cow.rmq2.cloudamqp.com/uunkvvsy");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare(queue: "hello-queue",durable: true, exclusive: false, autoDelete: false);
            channel.ExchangeDeclare(exchange: "logs-direct", type: ExchangeType.Direct, durable: true);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}";
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue: queueName, exchange: "logs-direct", routingKey: routeKey, arguments: null);
            });

            Enumerable.Range(1, 50).ToList().ForEach(x => {
                LogNames logType = (LogNames) new Random().Next(1, 5);
                string message = $"log-type: {logType}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{logType}";

                channel.BasicPublish(exchange: "logs-direct", routingKey: routeKey, basicProperties: null, body: messageBody);
                Console.WriteLine($"Log gönderilmiştir : {message}");

            });

            Console.ReadLine();
        }
    }
}
