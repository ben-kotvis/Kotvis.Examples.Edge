using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Models
{
    public class PublisherSubscriptionHeartbeat
    {
        public string SubscriptionId { get; set; }
        public DateTimeOffset SubscriptionExpiration { get; set; }
    }
}
