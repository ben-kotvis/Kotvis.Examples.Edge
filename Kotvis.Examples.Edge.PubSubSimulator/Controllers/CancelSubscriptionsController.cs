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
    public class CancelSubscriptionsController : ControllerBase
    {
        private readonly StateManager _stateManager;
        private readonly ISchedulerService _schedulerService;
        public CancelSubscriptionsController(StateManager stateManager, ISchedulerService schedulerService)
        {
            _stateManager = stateManager;
            _schedulerService = schedulerService;
        }

        [HttpPost]
        public async Task<AcceptedResult> Post(CancelSubscriptionRequest cancelRequest, CancellationToken cancellationToken)
        {
            var originalRequest = _stateManager.GetRequest(cancelRequest.SubscriptionId);
            var schedulerCancelRequest = new SchedulerModel.CancelScheduleRequest()
            {
                ScheduleId = originalRequest.ScheduleId
            };

            await _schedulerService.CancelSchedule(schedulerCancelRequest, cancellationToken);

            _stateManager.Cancel(cancelRequest.SubscriptionId);

            Console.Out.WriteLine($"Subscription: {cancelRequest.SubscriptionId} cancelled");
            return Accepted();
        }
    }
}
