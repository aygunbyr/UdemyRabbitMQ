using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

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

            channel.ExchangeDeclare(exchange: "header-exchange", type: ExchangeType.Headers, durable: true);

            Dictionary<string,object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true; // mesajları kalıcı hale getir

            Product product = new()
            {
                Id = 1,
                Name = "Pencil",
                Price = 100,
                Stock = 10
            };

            var productJsonString = JsonSerializer.Serialize(product);

            // Resim, pdf her şeyi byte array'e dönüştürebilirsiniz.
            byte[] message = Encoding.UTF8.GetBytes(productJsonString);

            channel.BasicPublish(exchange: "header-exchange", routingKey: string.Empty, basicProperties: properties, body: message);

            Console.WriteLine("Message has sent");

            Console.ReadLine();
        }
    }
}
