using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Threading.Tasks;
using Kotvis.Examples.Edge.Actor.Model;
using Dapr.Actors.Client;
using Dapr.Actors;
using Kotvis.Examples.Edge.Models;

namespace Kotvis.Examples.Edge.Subscriber
{
    internal class DeviceConnectionTracker : IDeviceConnectionTracker
    {
        public async Task Add(Publisher publisher)
        {
            var proxy = ActorProxy.Create<ISimulatedPublisherActor>(new ActorId(publisher.Id), "SimulatedPublisherActor");
            await proxy.Connect();
        }
        public Task Remove(string publisherId)
        {
            throw new NotImplementedException();
        }

        public async Task SendTelemetry(Publisher publisher, PublisherTelemetryMessage message)
        {
            var proxy = ActorProxy.Create<ISimulatedPublisherActor>(new ActorId(publisher.Id), "SimulatedPublisherActor");
            await proxy.SendTelemetry(message);

        }


    }

}
