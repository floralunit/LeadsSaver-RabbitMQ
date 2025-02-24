namespace LeadsSaverRabbitMQ.MessageModels;

public class RabbitMQStatusMessage_LMS
{
    public Guid Message_ID { get; set; }
    public Guid Center_ID { get; set; }
    public string BrandCenterName { get; set; }
}
