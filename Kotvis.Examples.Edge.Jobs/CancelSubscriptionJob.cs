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
    public class CancelSubscriptionJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly Publisher _publisher;
        public CancelSubscriptionJob(JobDependencyLocator jobDependencies, Publisher publisher)
        {
            _jobDependencies = jobDependencies;
            _publisher = publisher;
        }

        public async Task Run()
        {
            await _jobDependencies.PublisherApiService.CancelSubscription(_publisher, _jobDependencies.CancellationToken);
            _publisher.SubscriptionId = default;
            _publisher.ActualState = ActualPublisherState.StandingBy;

            var schedulerRequest = new SchedulerCancelRequest()
            {
                ScheduleId = _publisher.HealthScheduleId,
            };

            await _jobDependencies.SchedulerService.CancelSchedule(schedulerRequest, _jobDependencies.CancellationToken);


            Console.Out.WriteLine($"Cancelling subscription for publisher id: {_publisher.Id}");
        }
    }
}
