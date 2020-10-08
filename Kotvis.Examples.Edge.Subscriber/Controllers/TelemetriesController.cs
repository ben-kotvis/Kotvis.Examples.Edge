using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Jobs;
using Kotvis.Examples.Edge.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kotvis.Examples.Edge.Subscriber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelemetriesController : ControllerBase
    {
        private readonly JobDependencyLocator _jobDependencyLocator;
        public TelemetriesController(JobDependencyLocator jobDependencyLocator)
        {
            _jobDependencyLocator = jobDependencyLocator;
        }

        [HttpPost]
        public async Task<AcceptedResult> Post(PublisherTelemetryMessage telemetryMessage)
        {
            var job = new TelemetryReceivedJob(_jobDependencyLocator, telemetryMessage);
            await job.Run();
            return Accepted();
        }

    }
}
