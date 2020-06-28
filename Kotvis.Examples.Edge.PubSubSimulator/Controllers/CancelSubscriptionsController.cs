using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelSubscriptionsController : ControllerBase
    {
        private readonly StateManager _stateManager;
        public CancelSubscriptionsController(StateManager stateManager)
        {
            _stateManager = stateManager;
        }

        [HttpPost]
        public AcceptedResult Post(CancelSubscriptionRequest cancelRequest)
        {
            //_stateManager.CancelTask(cancelRequest.SubscriptionId);
            Console.Out.WriteLine($"Subscription: {cancelRequest.SubscriptionId} cancelled");
            return Accepted();
        }
    }
}
