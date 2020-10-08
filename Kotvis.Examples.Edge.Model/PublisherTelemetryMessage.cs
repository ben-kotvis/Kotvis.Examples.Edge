using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Models
{
    public class PublisherTelemetryMessage
    {
        public string SubscriptionId { get; set; }
        public DateTimeOffset MessageTime { get; set; }
        public int Pressure { get; set; }
    }
}
