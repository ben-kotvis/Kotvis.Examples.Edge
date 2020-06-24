using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Jobs;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kotvis.Examples.Edge.Subscriber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeartbeatsController : ControllerBase
    {
        private readonly JobDependencyLocator _jobDependencyLocator;
        public HeartbeatsController(JobDependencyLocator jobDependencyLocator)
        {
            _jobDependencyLocator = jobDependencyLocator;
        }

        [HttpPost]
        public async Task<AcceptedResult> Post(PublisherHeartbeat publisherHeartbeat)
        {
            var job = new HeartbeatReceivedJob(_jobDependencyLocator, publisherHeartbeat);
            await job.Run();
            return Accepted();
        }

        [HttpGet]
        public Task<OkObjectResult> Get()
        {
            var obj = new
            {
                Result = true
            };

            return Task.FromResult(Ok(obj));
        }
    }
}
