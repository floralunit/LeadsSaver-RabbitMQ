namespace LeadsSaverRabbitMQ.Configuration;

public class BrandConfigurationSettings
{
    public List<BrandConfiguration> Brands { get; set; }
}

public class BrandConfiguration
{
    public string BrandName { get; set; }
    public Guid ProjectGuid { get; set; }
    public string Url { get; set; }
    public List<CenterConfiguration> Centers { get; set; }
}

public class CenterConfiguration
{
    public string CenterName { get; set; }
    public Guid CenterGuid { get; set; }
    public string Token { get; set; }
}