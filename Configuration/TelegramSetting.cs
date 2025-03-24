using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadsSaver_RabbitMQ.Configuration
{
    public class TelegramOptions
    {
        public string BotToken { get; set; }
        public List<string> ChatIds { get; set; }
    }
}
