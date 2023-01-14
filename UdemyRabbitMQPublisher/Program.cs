using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace UdemyRabbitMQPublisher
{
    public enum LogName
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://yotwwirn:i0FRbWp-YGPUvlfwJWVDmrJgo_gWg_6t@hawk.rmq.cloudamqp.com/yotwwirn");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);



            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;

            var product = new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 100 };
            var productJsonString = JsonSerializer.Serialize(product);


            channel.BasicPublish("header-exchange", string.Empty, properties,
                Encoding.UTF8.GetBytes(productJsonString));

            Console.WriteLine("Mesaj oluştu");
            Console.ReadLine();
        }
    }
}
