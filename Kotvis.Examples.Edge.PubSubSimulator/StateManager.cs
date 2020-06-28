using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator
{
    public class StateManager 
    {
        private readonly IDictionary<string, SubscribeRequest> _subscriptionLookup;

        public StateManager()
        {
            _subscriptionLookup = new Dictionary<string, SubscribeRequest>();
        }

        public void AddTask(string id, SubscribeRequest subscribeRequest)
        {
            _subscriptionLookup.Add(id, subscribeRequest);
        }

        public SubscribeRequest GetRequest(string id)
        {
            return _subscriptionLookup[id];
        }
    }
}
