using CQRSMeetup.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.RabbitMq
{
    public class RabbitMqService : IRabbitMqService
    {
        public readonly IOptions<RabbitMqConfigModel> _rabbitMqConfig;
        public RabbitMqService(IOptions<RabbitMqConfigModel> rabbitMqConfig)
        {
            _rabbitMqConfig = rabbitMqConfig;
        }

        public bool Publish(string channelName, object data)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMqConfig.Value.RabbitMqHostname,
                    Password = _rabbitMqConfig.Value.RabbitMqPassword,
                    UserName = _rabbitMqConfig.Value.RabbitMqUsername
                };
                using (IConnection connection = factory.CreateConnection())
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: channelName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    string message = JsonConvert.SerializeObject(data);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: channelName,
                                         basicProperties: null,
                                         body: body);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}