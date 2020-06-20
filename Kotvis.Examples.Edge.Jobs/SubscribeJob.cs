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
        private readonly DesiredPublisher _publisher;
        public SubscribeJob(JobDependencyLocator jobDependencies, DesiredPublisher publisher)
        {
            _jobDependencies = jobDependencies;
            _publisher = publisher;
        }

        public async Task Run()
        {
            await _jobDependencies.PublisherApiService.Subscribe(_publisher);
            _publisher.State = PublisherState.Subscribed;
        }
    }
}
