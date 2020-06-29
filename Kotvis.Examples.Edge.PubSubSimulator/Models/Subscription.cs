using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Models
{
    public class Subscription
    {
        public string ScheduleId { get; set; }

        public SubscribeRequest Request { get; set; }
    }
}
