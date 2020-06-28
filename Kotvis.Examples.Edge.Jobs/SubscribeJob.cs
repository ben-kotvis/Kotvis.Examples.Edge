using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

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
            Monitor.Enter(_publisher);

            var subscriptionId = await _jobDependencies.PublisherApiService.Subscribe(_publisher, _jobDependencies.CancellationToken);
            _publisher.ActualState = ActualPublisherState.Subscribed;
            _publisher.SubscriptionId = subscriptionId;

            var context = new ElapsedScheduleMessage()
            {
                Context = subscriptionId,
                ScheduleId = Guid.NewGuid().ToString(),
                JobName = Constants.JobNames.HealthCheck
            };

            var schedulerRequest = new SchedulerRequest()
            {
                OutputName = Constants.Inputs.SubscriberInbound,
                Repeat = true,
                RunTime = TimeSpan.FromSeconds(30),
                Context = context
            };

            await _jobDependencies.SchedulerService.ScheduleJob(schedulerRequest, _jobDependencies.CancellationToken);

            _publisher.HealthScheduleId = context.ScheduleId;

            Console.Out.WriteLine($"Subscription: {subscriptionId} created for {_publisher.Id}");
            Monitor.Exit(_publisher);
        }
    }
}
