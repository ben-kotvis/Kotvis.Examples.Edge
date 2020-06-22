using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class ReportedPropertyUpdateJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        public ReportedPropertyUpdateJob(JobDependencyLocator jobDependencies)
        {
            _jobDependencies = jobDependencies;
        }

        public async Task Run()
        {
            if(!_jobDependencies.Module.IsChanged && !_jobDependencies.Module.Publishers.Any(i => i.IsChanged))
            {
                return;
            }

            var desired = new Dictionary<string, object>();
            desired.Add(Constants.TwinKeys.ModuleState, _jobDependencies.Module.State);

            foreach (var publisher in _jobDependencies.Module.Publishers.Where(i => i.IsChanged))
            {  
                desired.Add($"{Constants.TwinKeys.PublisherPrefix}{publisher.Id}", publisher);
            }

            var twin = new Dictionary<string, object>();
            twin.Add(Constants.TwinKeys.Module, desired);            

            await _jobDependencies.EdgeService.SetTwinReportedState(twin, _jobDependencies.CancellationToken);

            foreach (var publisher in _jobDependencies.Module.Publishers.Where(i => i.IsChanged))
            {
                publisher.AcceptChanges();
            }
            _jobDependencies.Module.AcceptChanges();
        }
    }
}
