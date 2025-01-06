using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://uunkvvsy:whtUMIENsS2ct10W6LctKFwoTNqtKyfJ@cow.rmq2.cloudamqp.com/uunkvvsy");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare(queue: "hello-queue",durable: true, exclusive: false, autoDelete: false);
            channel.ExchangeDeclare(exchange: "logs-fanout", type: ExchangeType.Fanout, durable: true);

            Enumerable.Range(1, 50).ToList().ForEach(x => {
                string message = $"log {x}";

                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "logs-fanout", routingKey: string.Empty, basicProperties: null, body: messageBody);
                Console.WriteLine($"Mesaj gönderilmiştir : {message}");

            });

            Console.ReadLine();
        }
    }
}
