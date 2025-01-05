using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.Subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://uunkvvsy:whtUMIENsS2ct10W6LctKFwoTNqtKyfJ@cow.rmq2.cloudamqp.com/uunkvvsy");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // global: true => prefetchCount 6 ise 2 subscriber varsa => 3+3 mesaj gider
            // global: false ise ve prefetchCount 1 ise => her bir subscribera birer birer gider
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // Exchange kullanılmıyor default exchange
            // Subscriberlar mesajları birer birer alıyor, mesajlar çoğaltılmıyor. Bir mesajı sadece bir subscriber alıyor.

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: "hello-queue", autoAck: false, consumer: consumer);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);
                Console.WriteLine($"Gelen Mesaj: {message}");

                channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            };

            Console.ReadLine();
        }
    }
}
