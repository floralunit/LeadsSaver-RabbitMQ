﻿namespace LeadsSaverRabbitMQ.MessageModels;

public class RabbitMQLeadMessage_DataMart
{
    public Guid Message_ID { get; set; }
    public Guid Center_ID { get; set; }
    public Guid Project_ID { get; set; }
}
