using MassTransit;
using LeadsSaverRabbitMQ.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using LeadsSaverRabbitMQ.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using LeadsSaverRabbitMQ.MessageModels;

namespace LeadsSaver_RabbitMQ.Consumers;

public class LeadsDataMartConsumer : IConsumer<RabbitMQLeadMessage_DataMart>
{
    private readonly AstraContext _dbContext;
    private readonly BrandConfigurationSettings _brandSettings;
    private readonly ILogger<LeadsDataMartConsumer> _logger;

    private readonly IPublishEndpoint _publishEndpoint;

    public LeadsDataMartConsumer(AstraContext dbContext, 
                            IOptions<BrandConfigurationSettings> brandSettings, 
                            ILogger<LeadsDataMartConsumer> logger,
                            IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _brandSettings = brandSettings.Value;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_DataMart> context)
    {
        _logger.LogInformation("NEW DATAMART MESSAGE Received: DATAMART Message ({Message}))", context.Message.Message_ID);
    }

}