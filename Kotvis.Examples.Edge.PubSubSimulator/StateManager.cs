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
        private readonly IDictionary<string, Subscription> _subscriptionLookup;

        public StateManager()
        {
            _subscriptionLookup = new Dictionary<string, Subscription>();
        }

        public void AddSubscription(string id, Subscription subscribeRequest)
        {
            _subscriptionLookup.Add(id, subscribeRequest);
        }

        public void Cancel(string id)
        {
            _subscriptionLookup.Remove(id);
        }

        public Subscription GetRequest(string id)
        {
            return _subscriptionLookup[id];
        }
    }
}
