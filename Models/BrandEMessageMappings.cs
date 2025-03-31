using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadsSaver_RabbitMQ.Models
{
    public class BrandEMessageMappings
    {
        public string Brand { get; set; }
        public int RequestTypeId { get; set; }
        public Guid VisitAimId { get; set; }
        public string VisitAimName { get; set; }
        public Guid EMessageSubjectId { get; set; }
        public string EMessageSubjectName { get; set; }
    }

}
