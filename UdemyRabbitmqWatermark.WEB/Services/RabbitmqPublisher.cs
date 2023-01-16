using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UdemyRabbitmqWatermark.WEB.Services
{
    public class RabbitmqPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitmqPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }
        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitMQClientService.Connect();
            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(RabbitMQClientService.ExchangeName, routingKey:
                RabbitMQClientService.RoutingWaterMark, basicProperties: properties,
                body: bodyByte);
        }
    }
}
