using Kotvis.Edge.Scheduler.Model;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Model = Kotvis.Examples.Edge.Model;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly IEdgeService _edgeService;
        public SchedulerService(IEdgeService edgeService)
        {
            _edgeService = edgeService;
        }
        public async Task ScheduleJob(SchedulerRequest schedulerRequest, CancellationToken cancellationToken)
        {
            await _edgeService.SendMessageToOutput(
                Model.Constants.Outputs.Scheduler, 
                JsonConvert.SerializeObject(schedulerRequest), 
                cancellationToken,
                NameValue.Create(Model.Constants.PropertyNames.RequestType, Model.Constants.Scheduler.ScheduleRequest));
        }

        public async Task CancelSchedule(CancelScheduleRequest schedulerCancelRequest, CancellationToken cancellationToken)
        {
            await _edgeService.SendMessageToOutput(
                Model.Constants.Inputs.CancelSchedule, 
                JsonConvert.SerializeObject(schedulerCancelRequest), 
                cancellationToken, 
                NameValue.Create(Model.Constants.PropertyNames.RequestType, Model.Constants.Scheduler.CancelRequest));
        }
    }
}
