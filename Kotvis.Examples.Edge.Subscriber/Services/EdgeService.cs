using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using Microsoft.Azure.Devices.Shared;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    internal class EdgeService : IEdgeService
    {
        private readonly ModuleClient _moduleClient;
        
        public EdgeService(ModuleClient moduleClient)
        {
            _moduleClient = moduleClient;
        }

        public async Task SendMessageToOutput(string outputName, string output, CancellationToken cancellationToken, params NameValue[] properties)
        {
            var message = new Message(Encoding.UTF8.GetBytes(output));

            if(properties != default)
            {
                foreach(var property in properties)
                {
                    message.Properties.Add(property.Name, property.Value);
                }
            }
            await _moduleClient.SendEventAsync(outputName, message, cancellationToken);
        }

        public async Task SetTwinReportedState(Dictionary<string, object> twinDictionary, CancellationToken cancellationToken)
        {
            var jObject = JObject.FromObject(twinDictionary);
            await _moduleClient.UpdateReportedPropertiesAsync(new TwinCollection(jObject.ToString()), cancellationToken);
        }
    }
}
