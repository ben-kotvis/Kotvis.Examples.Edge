using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.Models;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class TelemetryReceivedJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly PublisherTelemetryMessage _telemetryMessage;
        public TelemetryReceivedJob(JobDependencyLocator jobDependencies, PublisherTelemetryMessage telemetryMessage)
        {
            _jobDependencies = jobDependencies;
            _telemetryMessage = telemetryMessage;
        }

        public async Task Run()
        {

            var existing = _jobDependencies.Module.Publishers.FirstOrDefault(i => i.SubscriptionId == _telemetryMessage.SubscriptionId);
            if (existing == default)
            {
                Console.Out.WriteLine("Telemetry was received from an unknown subscription id");
                return;
            }

            Console.Out.WriteLine($"Telemetry was received for publisher id: {existing.Id}");
            existing.LastMessageTime = DateTimeOffset.UtcNow;


            await _jobDependencies.ConnectionTracker.SendTelemetry(existing, _telemetryMessage);

            await _jobDependencies.EdgeService.SendMessageToOutput(Constants.Outputs.TelemetryUpstream, JsonConvert.SerializeObject(_telemetryMessage), _jobDependencies.CancellationToken);

        }

    }
}
