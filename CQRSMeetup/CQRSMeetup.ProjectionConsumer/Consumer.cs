using CQRSMeetup.Core.Enums;
using CQRSMeetup.Core.Models;
using CQRSMeetup.Core.Redis;
using CQRSMeetup.ReadDomain;
using CQRSMeetup.WriteDomain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CQRSMeetup.ProjectionConsumer
{
    public class Consumer
    {
        private readonly RedisConfigModel _redisConfig;
        private readonly RabbitMqConfigModel _rabbitMqConfig;
        private readonly IRedisService _redisService;

        public Consumer(
            IOptions<RedisConfigModel> redisConfig,
            IOptions<RabbitMqConfigModel> rabbitMqConfig,
            IRedisService redisService)
        {
            _redisConfig = redisConfig?.Value ?? throw new ArgumentNullException(nameof(redisConfig));
            _rabbitMqConfig = rabbitMqConfig?.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfig));
            _redisService = redisService;
        }

        public async Task Run(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqConfig.RabbitMqHostname,
                UserName = _rabbitMqConfig.RabbitMqUsername,
                Password = _rabbitMqConfig.RabbitMqPassword
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "StockQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.Span;
                    var data = Encoding.UTF8.GetString(body);
                    var stockEntity = JsonConvert.DeserializeObject<StockEntity>(data);
                    var stockModel = new StockModel
                    {
                        Id = stockEntity.Id,
                        LocationId = stockEntity.LocationId,
                        LocationCode = stockEntity.Location?.Code,
                        LocationDescription = stockEntity.Location?.Description,
                        ProductId = stockEntity.ProductId,
                        ProductCode = stockEntity.Product?.Code,
                        ProductDescription = stockEntity.Product?.Description,
                        ProductSerialNo = stockEntity.Product?.SerialNo,
                        CreateDate = stockEntity.CreateDate,
                        UpdateDate = stockEntity.UpdateDate,
                        Status = stockEntity.Status,
                        Quantity = stockEntity.Quantity
                    };
                    if (stockModel.Status == RecordStatuses.PASSIVE)
                    {
                        _redisService.Remove($"stock:{ stockModel.Id}");
                    }
                    else
                    {
                        _redisService.Set($"stock:{ stockModel.Id}", stockModel);
                    }
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: "StockQueue",
                                     autoAck: true,
                                     consumer: consumer);
                Console.WriteLine(" press X for shotdown.");
                Console.ReadLine();

            }
        }
    }
}
