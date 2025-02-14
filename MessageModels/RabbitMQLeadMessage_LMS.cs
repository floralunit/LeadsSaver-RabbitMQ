namespace LeadsSaverRabbitMQ.MessageModels;

public class RabbitMQLeadMessage_LMS
{
    public Guid Message_ID { get; set; }
    public Guid Center_ID { get; set; }
    public string BrandName { get; set; }
}
