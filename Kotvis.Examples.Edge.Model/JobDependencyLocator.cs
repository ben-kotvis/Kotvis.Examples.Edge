using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kotvis.Examples.Edge.Model
{
    public class JobDependencyLocator
    {
        public JobDependencyLocator(
            IEdgeService edgeService,
            ISchedulerService schedulerService,
            IPublisherApiService publisherApiService,
            Module module,
            CancellationTokenSource cancellationTokenSource)
        {
            EdgeService = edgeService;
            SchedulerService = schedulerService;
            PublisherApiService = publisherApiService;
            Module = module;
            CancellationTokenSource = cancellationTokenSource;
        }

        public IEdgeService EdgeService { get; }
        public ISchedulerService SchedulerService { get; }
        public IPublisherApiService PublisherApiService { get; }
        public Module Module { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public CancellationToken CancellationToken => CancellationTokenSource.Token;
    }
}
