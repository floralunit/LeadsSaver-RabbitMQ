using System;

namespace LeadsSaver_RabbitMQ.Models
{
    public class EMessageResponsibleWorker
    {
        public Guid? OuterMessage_ID { get; set; }
        public Guid? ResponsibleWorker_ID { get; set; }
        public Guid? EMessage_ID { get; set; }
        public string ResponsibleName { get; set; }
        public bool SendStatus { get; set; }
    }
}
