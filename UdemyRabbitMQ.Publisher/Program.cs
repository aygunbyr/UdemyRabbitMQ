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
            channel.ExchangeDeclare(exchange: "logs-topic", type: ExchangeType.Topic, durable: true);

            Enumerable.Range(1, 50).ToList().ForEach(x => {
                LogNames logType = (LogNames) new Random().Next(1, 5);
                

                Random rnd = new();
                LogNames log1 = (LogNames)rnd.Next(1, 5);
                LogNames log2 = (LogNames)rnd.Next(1, 5);
                LogNames log3 = (LogNames)rnd.Next(1, 5);

                var routeKey = $"{log1}.{log2}.{log3}";
                string message = $"log-type: {log1}-{log2}-{log3}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "logs-topic", routingKey: routeKey, basicProperties: null, body: messageBody);
                Console.WriteLine($"Log gönderilmiştir : {message}");

            });

            Console.ReadLine();
        }
    }
}
