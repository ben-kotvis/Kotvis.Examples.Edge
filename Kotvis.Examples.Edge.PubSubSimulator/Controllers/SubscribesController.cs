using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribesController : ControllerBase
    {
        private readonly StateManager _stateManager;
        private readonly ISchedulerService _schedulerService;
        public SubscribesController(StateManager stateManager, ISchedulerService schedulerService)
        {
            _stateManager = stateManager;
            _schedulerService = schedulerService;
        }

        [HttpPost]
        public async Task<CreatedResult> Post(SubscribeRequest subscribeRequest, CancellationToken cancellationToken)
        {
            var subscriptionId = Guid.NewGuid().ToString();
            var scheduleId = Guid.NewGuid().ToString(); 

            await _schedulerService.ScheduleJob(CreateRequest(subscriptionId, scheduleId, Constants.JobNames.PubSubHeartbeatJob), cancellationToken);
            await _schedulerService.ScheduleJob(CreateRequest(subscriptionId, scheduleId, Constants.JobNames.PubSubTelemetryJob), cancellationToken);

            var request = new Subscription()
            {
                Request = subscribeRequest,
                ScheduleId = scheduleId
            };

            _stateManager.AddSchedule(subscriptionId, request);                

            var response = new PubSubSimulator.Models.SubscribeResponse()
            {
                SubscriptionId = subscriptionId
            };

            Console.Out.WriteLine($"Subscription: {subscriptionId} created");

            return Created("/hello", response);
        }

        private SchedulerRequest CreateRequest(string subscriptionId, string scheduleId, string jobName)
        {
            var scheduleReqeust = new SchedulerRequest();
            scheduleReqeust.OutputName = Constants.Outputs.PubSubSimulator;
            scheduleReqeust.Repeat = true;
            scheduleReqeust.RunTime = TimeSpan.FromSeconds(15);
            scheduleReqeust.Context = new ElapsedScheduleMessage()
            {
                Context = subscriptionId,
                JobName = jobName,
                ScheduleId = scheduleId
            };
            return scheduleReqeust;
        }
    }
}
