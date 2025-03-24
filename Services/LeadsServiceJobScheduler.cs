using Quartz;
using LeadsSaver_RabbitMQ.Jobs;
using Microsoft.Extensions.Hosting;

public class LeadsServiceJobScheduler : IHostedService
{
    private readonly IScheduler _scheduler;

    public LeadsServiceJobScheduler(
        IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _scheduler.Start(cancellationToken);


        var job = JobBuilder.Create<SendErrorLeadsToTelegramJob>()
            .WithIdentity($"sendErrorLeadsToTelegramJob", "groupsendErrorLeads")
            .UsingJobData("test", "test")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"sendErrorLeadsToTelegramJob", "groupsendErrorLeads")
            .StartNow()
            //.WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromMinutes(30)).RepeatForever())
            .WithCronSchedule("0 0/30 9-18 ? * * *", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"))) // Запуск каждые 30 минут с 9 утра до 6 вечера
            .Build();

        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
