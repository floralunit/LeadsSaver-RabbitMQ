using LeadsSaver_RabbitMQ.Consumers;
using LeadsSaverRabbitMQ.Configuration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LeadsSaverRabbitMQ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<RabbitMqSettings>(hostContext.Configuration.GetSection("RabbitMqSettings"));
                    services.Configure<BrandConfigurationSettings>(hostContext.Configuration.GetSection("BrandSettings"));

                    services.AddDbContext<AstraContext>(options =>
                    {
                        options
                        .UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"))
                        .LogTo(Console.WriteLine, LogLevel.Information);
                    });
                    services.AddMassTransit(cfg =>
                    {
                        // Add bus to the collection
                        cfg.AddBus(ConfigureBus);
                        // Add consumer to the collection
                        cfg.AddConsumer<LeadsLMSConsumer>();
                    });

                    // Add IHostedService registration of type BusService
                    services.AddHostedService<BusService>();
                    services.AddMassTransitHostedService();
                });

        private static IBusControl ConfigureBus(IServiceProvider provider)
        {
            var rabbitMqSettings = provider.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ReceiveEndpoint(rabbitMqSettings.QueueName, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.Consumer<LeadsLMSConsumer>(provider);
                });
            });
        }
    }
}