namespace LeadsSaverRabbitMQ.Configuration;

public class RabbitMqSettings
{
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string QueueName_SendLeads_LMS { get; set; }
    public string QueueName_SendStatus_LMS { get; set; }
    public string QueueName_SendLeads_LMP { get; set; }
    public string QueueName_SendStatus_LMP { get; set; }
    public string QueueName_SendLeads_DataMart { get; set; }
    public string QueueName_SendStatus_DataMart { get; set; }
}
