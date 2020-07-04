using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SchedulerModel = Kotvis.Edge.Scheduler.Model;

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

            var heartBeatScheduleReqeust = new SchedulerModel.SchedulerRequest();
            heartBeatScheduleReqeust.OutputName = Constants.Outputs.PubSubSimulator;
            heartBeatScheduleReqeust.Repeat = true;
            heartBeatScheduleReqeust.RunTime = TimeSpan.FromSeconds(15);
            heartBeatScheduleReqeust.ScheduleId = scheduleId;
            heartBeatScheduleReqeust.Context = new SchedulerModel.ElapsedScheduleMessage()
            {
                Context = subscriptionId,
                JobName = Constants.JobNames.PubSubHeartbeatJob,
                CorrelationId = subscriptionId
            };

            await _schedulerService.ScheduleJob(heartBeatScheduleReqeust, cancellationToken);

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
    }
}
