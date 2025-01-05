using RabbitMQ.Client;
using System;
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

            channel.QueueDeclare(
                queue: "hello-queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            string message = "hello world";

            var messageBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty, routingKey: "hello-queue", basicProperties: null, body: messageBody);

            Console.WriteLine("Mesaj gönderilmiştir");

            Console.ReadLine();
        }
    }
}
