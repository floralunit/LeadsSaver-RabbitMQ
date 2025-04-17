
namespace LeadsSaverRabbitMQ.Models
{
    public class EMessage
    {
        public Guid? DocumentBase_ID { get; set; }
        public Guid? DocumentAllowedState_ID { get; set; }
        public string? ResponsibleName { get; set; }
        public Guid? OuterMessage_ID { get; set; }

    }
}
