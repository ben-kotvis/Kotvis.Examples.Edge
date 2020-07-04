using Kotvis.Edge.Scheduler.Model;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model = Kotvis.Examples.Edge.Model;

namespace Kotvis.Examples.Edge.PubSubSimulator.Services
{
    public class SchedulerService : ISchedulerService
    {
        private readonly ModuleClient _moduleClient;
        public SchedulerService(ModuleClient moduleClient)
        {
            _moduleClient = moduleClient;
        }
        public async Task ScheduleJob(SchedulerRequest schedulerRequest, CancellationToken cancellationToken)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(schedulerRequest));
            var message = new Message(bytes);
            message.Properties.Add(Model.Constants.PropertyNames.RequestType, Model.Constants.Scheduler.ScheduleRequest);
            await _moduleClient.SendEventAsync(Model.Constants.Outputs.Scheduler, message, cancellationToken);
        }

        public async Task CancelSchedule(CancelScheduleRequest schedulerCancelRequest, CancellationToken cancellationToken)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(schedulerCancelRequest));
            var message = new Message(bytes);
            message.Properties.Add(Model.Constants.PropertyNames.RequestType, Model.Constants.Scheduler.CancelRequest);
            await _moduleClient.SendEventAsync(Model.Constants.Inputs.CancelSchedule, message, cancellationToken);
        }
    }
}
