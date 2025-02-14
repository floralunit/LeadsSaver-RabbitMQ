namespace LeadsSaverRabbitMQ.MessageModels;

public class RabbitMQStatusMessage_LMS
{
    public Guid Message_ID { get; set; }
    public int StageWorkId { get; set; }
    public int? ResultId { get; set; }
    public Guid Center_ID { get; set; }
    public string BrandName { get; set; }
}
