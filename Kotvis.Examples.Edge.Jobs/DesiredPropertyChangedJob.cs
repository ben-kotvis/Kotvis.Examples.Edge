using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Jobs
{
    public class DesiredPropertyChangedJob : IJob
    {
        private readonly JobDependencies _jobDependencies;
        private readonly TwinCollection _twinCollection;
        public DesiredPropertyChangedJob(JobDependencies jobDependencies, TwinCollection twinCollection)
        {
            _jobDependencies = jobDependencies;
            _twinCollection = twinCollection;
        }

        public async Task Run()
        {
            if(!_twinCollection.Contains(Constants.TwinKeys.Module))
            {
                await Console.Out.WriteLineAsync("Module not defined");
                return;
            }

            var module = _twinCollection[Constants.TwinKeys.Module];
            _jobDependencies.DesiredModule.State = module.State;

            _jobDependencies.DesiredModule.Publishers.Clear();

            foreach (var item in module.Publishers)
            {
                _jobDependencies.DesiredModule.Publishers.Add(item);
            }
        }

    }
}
