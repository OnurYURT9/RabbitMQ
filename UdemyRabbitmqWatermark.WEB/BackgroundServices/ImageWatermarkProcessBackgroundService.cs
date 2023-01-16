using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using UdemyRabbitmqWatermark.WEB.Services;

namespace UdemyRabbitmqWatermark.WEB.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMQClientService,
            ILogger<ImageWatermarkProcessBackgroundService> logger)
        {

            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
            }
        
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {

            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);
               return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cunsomer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, cunsomer);
            cunsomer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            Task.Delay(5000).Wait();
            try
            {
                var ProductImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>
             (Encoding.UTF8.GetString(@event.Body.ToArray()));

                var siteName = "www.mysite.com";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images",
                    ProductImageCreatedEvent.ImageName);
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);
                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold,
                    GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString(siteName, font);
                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);
                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height -
                    ((int)textSize.Height + 30));
                graphic.DrawString(siteName, font, brush, position);
                img.Save("wwwroot/images/watermarks/" + ProductImageCreatedEvent.ImageName);
                img.Dispose();
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.CompletedTask;

         

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
