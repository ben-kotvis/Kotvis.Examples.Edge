using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class AddPublisherMethodJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly string _payload;
        public AddPublisherMethodJob(JobDependencyLocator jobDependencies, string payload)
        {
            _jobDependencies = jobDependencies;
            _payload = payload;
        }

        public async Task Run()
        {
            try
            {
                Console.WriteLine($"Add publisher job called with payload {_payload}");
                var publisher = JsonConvert.DeserializeObject<Publisher>(_payload);
                publisher.PublisherActorId = Guid.NewGuid().ToString();

                _jobDependencies.Module.Publishers.Add(publisher);
                await _jobDependencies.ConnectionTracker.Add(publisher);
                var job = new SubscribeJob(_jobDependencies, publisher);
                await job.Run();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
