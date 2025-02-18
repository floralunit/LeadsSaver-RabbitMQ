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
        private readonly BrandConfigurationSettings _brandSettings;
        private readonly ILogger<BrandDbContextFactory> _logger;

        public BrandDbContextFactory(IConfiguration configuration, IOptions<BrandConfigurationSettings> brandSettings, ILogger<BrandDbContextFactory> logger)
        {
            _configuration = configuration;
            _brandSettings = brandSettings.Value;
            _logger = logger;
        }
    

        public AstraContext CreateDbContext(string brandName)
        {
            var brand = _brandSettings.Brands.FirstOrDefault(b => b.BrandName == brandName);
            if (brand == null)
            {
                _logger.LogError($"Бренд '{brandName}' не найден.", DateTimeOffset.Now);
                return null;
            }

            var connectionString = _configuration.GetConnectionString(brand.DataBase);
            var optionsBuilder = new DbContextOptionsBuilder<AstraContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AstraContext(optionsBuilder.Options);
        }
    }


}
