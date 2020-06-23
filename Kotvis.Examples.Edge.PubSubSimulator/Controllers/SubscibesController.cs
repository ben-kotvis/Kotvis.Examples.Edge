﻿using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscibesController : ControllerBase
    {
        private readonly StateManager _stateManager;
        private readonly SimulatedHeartbeat _simulatedHeartbeat;
        public SubscibesController(StateManager stateManager, SimulatedHeartbeat simulatedHeartbeat)
        {
            _stateManager = stateManager;
            _simulatedHeartbeat = simulatedHeartbeat;
        }

        [HttpPost]
        public Task<CreatedResult> Post(SubscribeRequest subscribeRequest)
        {
            var id = Guid.NewGuid().ToString();
            _stateManager.AddTask(id, async (token) =>  await _simulatedHeartbeat.Create(subscribeRequest.SubscriberAddress, id, token));
            return Task.FromResult(Created("", subscribeRequest));
        }
    }
}
