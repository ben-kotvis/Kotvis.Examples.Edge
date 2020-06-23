using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kotvis.Examples.Edge.Model;
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
        public Task<AcceptedResult> Post(object heartbeatRequest)
        {
            //var job = new ReceiveHeartbeatJob(_client, _module, heartbeatRequest);
            //await job.Run();

            return Task.FromResult(Accepted());
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
