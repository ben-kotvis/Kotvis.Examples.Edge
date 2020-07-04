using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class RoutingJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        public RoutingJob(JobDependencyLocator jobDependencies)
        {
            _jobDependencies = jobDependencies;
        }

        public async Task Run()
        {
            var taskList = new List<Task>();
            foreach(var publisher in _jobDependencies.Module.Publishers.Where(i => i.IsChanged && i.ActualState != ActualPublisherState.Error))
            {
                IJob job = default;
                switch(publisher.DesiredState)
                {
                    case DesiredPublisherState.Online:
                        job = CreateHealthyJob(publisher);
                        break;
                    case DesiredPublisherState.Removed:
                        job = CreateRemovedJob(publisher);
                        break;
                    case DesiredPublisherState.StandingBy:
                        job = CreateStandingByJob(publisher);
                        break;
                }

                if(job != default)
                {
                    taskList.Add(job.Run());
                }
            }

            await Task.WhenAll(taskList);
        }

        private IJob CreateStandingByJob(Publisher publisher)
        {
            switch(publisher.ActualState)
            {
                case ActualPublisherState.Default:
                case ActualPublisherState.StandingBy:
                case ActualPublisherState.Removed:
                    return default;
                default:
                    //credentials or host information changed so cancel
                    return new CancelSubscriptionJob(_jobDependencies, publisher);
            }
        }

        private IJob CreateRemovedJob(Publisher publisher)
        {
            if (publisher.ActualState != ActualPublisherState.Removed)
            {
                //credentials or host information changed so cancel
                return new CancelSubscriptionJob(_jobDependencies, publisher);
            }
            return default;
        }

        private IJob CreateHealthyJob(Publisher publisher)
        {
            if (publisher.ActualState == ActualPublisherState.Healthy)
            {
                //credentials or host information changed so resubscribe
                return new ResubscribeJob(_jobDependencies, publisher);
            }
            else
            {
                return new SubscribeJob(_jobDependencies, publisher);
            }
        }
    }
}
