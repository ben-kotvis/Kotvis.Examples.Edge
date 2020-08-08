using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class HeartbeatReceivedJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly PublisherHeartbeat _publisherHeartbeat;
        public HeartbeatReceivedJob(JobDependencyLocator jobDependencies, PublisherHeartbeat publisherHeartbeat)
        {
            _jobDependencies = jobDependencies;
            _publisherHeartbeat = publisherHeartbeat;
        }

        public async Task Run()
        {

            var existing = _jobDependencies.Module.Publishers.FirstOrDefault(i => i.SubscriptionId == _publisherHeartbeat.SubscriptionId);
            if (existing == default)
            {
                Console.Out.WriteLine("Heartbeat was received from an unknown subscription id");
                return;
            }


            Console.Out.WriteLine($"Heartbeat was received for publisher id: {existing.Id}");
            existing.LastMessageTime = DateTimeOffset.UtcNow;

            if (existing.ActualState != ActualPublisherState.Healthy)
            {
                existing.ActualState = ActualPublisherState.Healthy;
                var reportedPropertyJob = new ReportedPropertyUpdateJob(_jobDependencies);
                await reportedPropertyJob.Run();
            }

        }

    }
}
