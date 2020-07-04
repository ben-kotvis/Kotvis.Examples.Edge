using Kotvis.Edge.Scheduler.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface ISchedulerService
    {
        Task ScheduleJob(SchedulerRequest schedulerRequest, CancellationToken cancellationToken);
        Task CancelSchedule(CancelScheduleRequest schedulerCancelRequest, CancellationToken cancellationToken);
    }
}
