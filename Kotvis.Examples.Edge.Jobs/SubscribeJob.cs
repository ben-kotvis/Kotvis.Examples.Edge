using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class SubscribeJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly Publisher _publisher;
        public SubscribeJob(JobDependencyLocator jobDependencies, Publisher publisher)
        {
            _jobDependencies = jobDependencies;
            _publisher = publisher;
        }

        public async Task Run()
        {
            var subscriptionId = await _jobDependencies.PublisherApiService.Subscribe(_publisher, _jobDependencies.CancellationToken);
            _publisher.ActualState = ActualPublisherState.Subscribed;
            _publisher.SubscriptionId = subscriptionId;

            Console.Out.WriteLine($"Subscription: {subscriptionId} created for {_publisher.Id}");
        }
    }
}
