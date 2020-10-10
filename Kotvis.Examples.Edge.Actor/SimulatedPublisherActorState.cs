using Kotvis.Examples.Edge.Actor.Model;
using Kotvis.Examples.Edge.Model;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Azure.Devices.Client;
using System;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Kotvis.Examples.Edge.Models;

namespace Kotvis.Examples.Edge.Actor
{
    public class SimulatedPublisherActorState 
    {
        public const string StateName = "HeartbeatTime";

        public DateTimeOffset HeartbeatTime { get; set; }

        public string DeviceId { get; set; }
    }

}
