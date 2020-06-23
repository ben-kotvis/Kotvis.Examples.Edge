using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Models
{
    public class SubscribeRequest
    {
        public string SubscriberAddress { get; set; }
        public int SubscriberPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
