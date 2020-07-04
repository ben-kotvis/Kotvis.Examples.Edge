using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using SchedulerModel = Kotvis.Edge.Scheduler.Model;

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

            var context = new SchedulerModel.ElapsedScheduleMessage()
            {
                Context = subscriptionId,
                JobName = Constants.JobNames.HealthCheck,
                CorrelationId = subscriptionId
            };

            var schedulerRequest = new SchedulerModel.SchedulerRequest()
            {
                OutputName = Constants.Outputs.Subscriber,
                Repeat = true,
                RunTime = TimeSpan.FromSeconds(30),
                Context = context,
                ScheduleId = Guid.NewGuid().ToString(),
            };

            await _jobDependencies.SchedulerService.ScheduleJob(schedulerRequest, _jobDependencies.CancellationToken);

            _publisher.HealthScheduleId = schedulerRequest.ScheduleId;

            Console.Out.WriteLine($"Subscription: {subscriptionId} created for {_publisher.Id}");
        }
    }
}
