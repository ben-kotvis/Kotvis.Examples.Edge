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
            var id = Guid.NewGuid().ToString();
            var scheduleId = Guid.NewGuid().ToString();
            var heartBeatScheduleReqeust = new SchedulerRequest();
            heartBeatScheduleReqeust.OutputName = Constants.Inputs.SubscriberInbound;
            heartBeatScheduleReqeust.Repeat = true;
            heartBeatScheduleReqeust.RunTime = TimeSpan.FromSeconds(15);
            heartBeatScheduleReqeust.Context = new ElapsedScheduleMessage()
            {
                Context = id,
                JobName = Constants.JobNames.PubSubHeartbeatJob,
                ScheduleId = scheduleId
            };

            await _schedulerService.ScheduleJob(heartBeatScheduleReqeust, cancellationToken);

            _stateManager.AddTask(id, subscribeRequest);
                
                //$"http://{subscribeRequest.SubscriberAddress}:{subscribeRequest.SubscriberPort}/api/heartbeats", id, token));

            var response = new PubSubSimulator.Models.SubscribeResponse()
            {
                SubscriptionId = id
            };

            Console.Out.WriteLine($"Subscription: {id} created");

            return Created("/hello", response);
        }
    }
}
