using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMSWebService.Models;

public class RabbitMQLeadMessage
{
    public Guid Message_ID { get; set; }
    public Guid Center_ID { get; set; }
    public string BrandName { get; set; }
}
