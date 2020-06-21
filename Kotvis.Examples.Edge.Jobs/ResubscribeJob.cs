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
    public class ResubscribeJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly Publisher _publisher;
        public ResubscribeJob(JobDependencyLocator jobDependencies, Publisher publisher)
        {
            _jobDependencies = jobDependencies;
            _publisher = publisher;
        }

        public async Task Run()
        {
            var cancelJob = new CancelSubscriptionJob(_jobDependencies, _publisher);
            await cancelJob.Run();

            var job = new SubscribeJob(_jobDependencies, _publisher);
            await job.Run();
        }
    }
}
