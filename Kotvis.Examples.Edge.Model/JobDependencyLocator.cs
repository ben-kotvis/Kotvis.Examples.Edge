using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class JobDependencyLocator
    {
        public JobDependencyLocator(
            IEdgeService edgeService,
            ISchedulerService schedulerService,
            IPublisherApiService publisherApiService,
            Module module,
            DesiredModule desiredModule)
        {
            EdgeService = edgeService;
            SchedulerService = schedulerService;
            PublisherApiService = publisherApiService;
            Module = module;
            DesiredModule = desiredModule;
        }

        public IEdgeService EdgeService { get; }
        public ISchedulerService SchedulerService { get; }
        public IPublisherApiService PublisherApiService { get; }
        public Module Module { get; }
        public DesiredModule DesiredModule { get; }
    }
}
