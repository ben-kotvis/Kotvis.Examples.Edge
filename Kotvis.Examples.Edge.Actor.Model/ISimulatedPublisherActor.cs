using Dapr.Actors;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Models;
using System;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Actor.Model
{
    public interface ISimulatedPublisherActor : IActor
    {
        Task SendTelemetry(PublisherTelemetryMessage message);
        Task Connect(string deviceId);
        Task Heartbeat();
    }
}
