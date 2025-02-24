namespace LeadsSaverRabbitMQ.MessageModels;

public class RabbitMQLeadMessage_LMS
{
    public Guid Message_ID { get; set; }
    public string BrandCenterName { get; set; }
    public Guid Center_ID { get; set; }
    public Guid Project_ID { get; set; }
    public string Database { get; set; }
}
