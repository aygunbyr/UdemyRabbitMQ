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

            #region geçici kuyruk
            // Bu kuyruk geçicidir, consumer uygulaması kapatıldığında silinir.
            var randomQueueName = channel.QueueDeclare().QueueName;
            #endregion

            #region kalıcı kuyruk
            //var randomQueueName = "log-database-save-queue"; // Artık random kuyruk ismi kullanmıyoruz çünkü kalıcı yapacağız.
            // Kalıcı kuyruk oluşturuyoruz artık. Uygulama kapansa bile queue silinmiyor.
            //channel.QueueDeclare(queue: randomQueueName, durable: true, exclusive: false, autoDelete: false);
            #endregion

            channel.QueueBind(queue: randomQueueName, exchange: "logs-fanout", routingKey: string.Empty);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: randomQueueName, autoAck: false, consumer: consumer);

            Console.WriteLine("Loglar dinleniyor...");

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
