using LeadsSaverRabbitMQ.MessageModels;
using LeadsSaver_RabbitMQ.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using LeadsSaverRabbitMQ.Configuration;

namespace LeadsSaver_RabbitMQ.Jobs
{
    [DisallowConcurrentExecution]
    public class SendErrorLeadsToTelegramJob : IJob
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SendErrorLeadsToTelegramJob> _logger;
        private readonly IBrandDbContextFactory _dbContextFactory;
        private readonly TelegramOptions _telegramOptions;

        public SendErrorLeadsToTelegramJob(HttpClient httpClient,
                                //AstraContext dbContext,
                                ILogger<SendErrorLeadsToTelegramJob> logger,
                                IBrandDbContextFactory dbContextFactory,
                                IOptions<TelegramOptions> telegramOptions)
        {
            //_dbContext = dbContext;
            _httpClient = httpClient;
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _telegramOptions = telegramOptions.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("");
            _logger.LogInformation("");
            _logger.LogInformation($"Job {context.JobDetail.Key.Name} started", DateTimeOffset.Now);

            using var _dbContextAUDI = _dbContextFactory.CreateDbContext("ASTRA_AUDI");
            var errorLeads3A = await _dbContextAUDI.OuterMessage.Where(x => x.ProcessingStatus == 3).ToListAsync(); //не создались обращения

            foreach (var lead3A in errorLeads3A)
            {
                await SendMessageToTelegramBotAsync(lead3A.ErrorMessage);
            }

            using var _dbContextVNUKOVO = _dbContextFactory.CreateDbContext("CASH_FLOW_SKODA_O");
            var errorLeads3V = await _dbContextVNUKOVO.OuterMessage.Where(x => x.ProcessingStatus == 3).ToListAsync(); //не создались обращения

            foreach (var lead3V in errorLeads3V)
            {
                if (lead3V.ErrorMessage.Contains("request_type=40") || lead3V.ErrorMessage.Contains("request_type=619")) // не разобрались, что это за request_type, пропускаем, чтобы бот не спамил
                {
                    continue;
                }
                await SendMessageToTelegramBotAsync(lead3V.ErrorMessage);
            }

        }

        public async Task SendMessageToTelegramBotAsync(string message)
        {
            foreach (var chatId in _telegramOptions.ChatIds)
            {
                var requestUrl = $"https://api.telegram.org/bot{_telegramOptions.BotToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}&parse_mode=HTML";
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
            }
        }
    }


}
