using CQRSMeetup.Core.Models;
using CQRSMeetup.Core.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace CQRSMeetup.ProjectionConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", reloadOnChange: false, optional: false)
                .Build();

            IServiceCollection services = new ServiceCollection()
                                        .Configure<RedisConfigModel>(Configuration.GetSection("RedisConfig"))
                                        .Configure<RabbitMqConfigModel>(Configuration.GetSection("RabbitMqConfig"))
                                        .AddSingleton<IRedisService, RedisService>()
                                        .AddSingleton<Consumer>();


            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            await serviceProvider.GetService<Consumer>().Run(args);

        }
    }
}
