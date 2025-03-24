using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;

namespace LeadsSaver_RabbitMQ.Configuration
{

    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            //return _serviceProvider.GetRequiredService<IJob>();
        }

        public void ReturnJob(IJob job) { }
    }
}
