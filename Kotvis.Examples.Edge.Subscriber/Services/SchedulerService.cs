using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    public class SchedulerService : ISchedulerService
    {
        public Task ScheduleJob(SchedulerRequest schedulerRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
