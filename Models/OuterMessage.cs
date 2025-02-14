﻿
namespace LeadsSaverRabbitMQ.Models
{
    public class OuterMessage
    {
        public Guid OuterMessage_ID { get; set; }
        public int? OuterMessageReader_ID { get; set; }
        public string? MessageOuter_ID { get; set; }
        public byte? ProcessingStatus { get; set; }
        public string? MessageText { get; set; }
        public int? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? InsDate { get; set; }
        public DateTime? UpdDate { get; set; }
    }
}
