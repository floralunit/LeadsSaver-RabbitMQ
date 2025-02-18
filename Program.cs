using LeadsSaver_RabbitMQ.Consumers;
using LeadsSaverRabbitMQ.Configuration;
using LeadsSaverRabbitMQ.MessageModels;
using LeadsSaverRabbitMQ.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;

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
                        //options
                        //.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                        //.LogTo(Console.WriteLine, LogLevel.Information);
                    });
                    services.AddTransient<IBrandDbContextFactory, BrandDbContextFactory>();

                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddBus(ConfigureBus);
                        cfg.AddConsumer<LeadsLMSConsumer>();
                    });

                    // Add IHostedService registration of type BusService
                    services.AddHostedService<BusService>();
                    services.AddMassTransitHostedService();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddNLog();
                    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                    logging.AddFilter("Microsoft", LogLevel.Warning);
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

                // Указываем, что сообщения типа LeadMessage будут отправляться в конкретную очередь
                cfg.Message<RabbitMQStatusMessage_LMS>(x => x.SetEntityName(rabbitMqSettings.QueueName_SendStatus_LMS));

                // Настройка отправки сообщений в очередь
                EndpointConvention.Map<RabbitMQStatusMessage_LMS>(new Uri($"queue:{rabbitMqSettings.QueueName_SendStatus_LMS}"));

                cfg.Message<RabbitMQStatusMessage_LMP>(x => x.SetEntityName(rabbitMqSettings.QueueName_SendStatus_LMP));
                EndpointConvention.Map<RabbitMQStatusMessage_LMP>(new Uri($"queue:{rabbitMqSettings.QueueName_SendStatus_LMP}"));

                cfg.Message<RabbitMQStatusMessage_DataMart>(x => x.SetEntityName(rabbitMqSettings.QueueName_SendStatus_DataMart));
                EndpointConvention.Map<RabbitMQStatusMessage_DataMart>(new Uri($"queue:{rabbitMqSettings.QueueName_SendStatus_DataMart}"));

                cfg.ReceiveEndpoint(rabbitMqSettings.QueueName_SendLeads_LMS, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.Consumer<LeadsLMSConsumer>(provider);
                });

                cfg.ReceiveEndpoint(rabbitMqSettings.QueueName_SendLeads_LMP, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.Consumer<LeadsLMPConsumer>(provider);
                });

                cfg.ReceiveEndpoint(rabbitMqSettings.QueueName_SendLeads_DataMart, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, 100));

                    e.Consumer<LeadsDataMartConsumer>(provider);
                });
            });
        }
    }
}