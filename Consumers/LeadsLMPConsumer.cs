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

public class LeadsLMPConsumer : IConsumer<RabbitMQLeadMessage_LMP>
{
    private readonly AstraContext _dbContext;
    private readonly BrandConfigurationSettings _brandSettings;
    private readonly ILogger<LeadsLMPConsumer> _logger;

    private readonly IPublishEndpoint _publishEndpoint;

    public LeadsLMPConsumer(AstraContext dbContext, 
                            IOptions<BrandConfigurationSettings> brandSettings, 
                            ILogger<LeadsLMPConsumer> logger,
                            IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _brandSettings = brandSettings.Value;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<RabbitMQLeadMessage_LMP> context)
    {
        _logger.LogInformation("NEW LMP MESSAGE Received: LMP Message ({Message}))", context.Message.Message_ID);

    }
}