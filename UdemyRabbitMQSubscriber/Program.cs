using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace UdemyRabbitMQSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://yotwwirn:i0FRbWp-YGPUvlfwJWVDmrJgo_gWg_6t@hawk.rmq.cloudamqp.com/yotwwirn");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false); //mesajları kaçar tane gönderdiğini belirledik
            var consumer = new EventingBasicConsumer(channel);
            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "all"); //bütün key value çiftlerinin eşleşmesi lazım

            channel.QueueBind(queueName, "header-exchange", String.Empty);
            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Product product = JsonSerializer.Deserialize<Product>(message);
                Thread.Sleep(1500);
                Console.WriteLine($"Gelen mesaj + { product.Id}- {product.Name}-{product.Price}-{product.Stock}");
                // File.AppendAllText("log-critical.txt", message+ "\n");
                channel.BasicAck(e.DeliveryTag, false);

            };

            Console.ReadLine();
        }
    }
}
