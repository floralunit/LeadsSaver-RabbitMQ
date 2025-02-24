using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LeadsSaverRabbitMQ.Configuration
{
    public interface IBrandDbContextFactory
    {
        AstraContext CreateDbContext(string brand);
    }

    public class BrandDbContextFactory : IBrandDbContextFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BrandDbContextFactory> _logger;

        public BrandDbContextFactory(IConfiguration configuration, ILogger<BrandDbContextFactory> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
    

        public AstraContext CreateDbContext(string database)
        {
            var connectionString = _configuration.GetConnectionString(database);
            var optionsBuilder = new DbContextOptionsBuilder<AstraContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AstraContext(optionsBuilder.Options);
        }
    }


}
