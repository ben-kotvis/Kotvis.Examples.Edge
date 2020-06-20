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
    public class DesiredPropertyChangedJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly TwinCollection _twinCollection;
        public DesiredPropertyChangedJob(JobDependencyLocator jobDependencies, TwinCollection twinCollection)
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

            JObject module = _twinCollection[Constants.TwinKeys.Module];
            _jobDependencies.DesiredModule.State = (ModuleState)module.SelectToken(Constants.TwinKeys.ModuleState).Value<Int32>();

            _jobDependencies.DesiredModule.Publishers.Clear();

            var subscribeJobTasks = new List<Task>();

            foreach (JProperty item in module.Children().Where(i => i.Path.Contains(Constants.TwinKeys.PublisherPrefix)))
            {
                DesiredPublisher desiredPublisher;
                if (TryParsePublisher(item, out desiredPublisher))
                {
                    _jobDependencies.DesiredModule.Publishers.Add(desiredPublisher);
                    subscribeJobTasks.Add(new SubscribeJob(_jobDependencies, desiredPublisher).Run());
                }
            }

            await Task.WhenAll(subscribeJobTasks);
        }

        private bool TryParsePublisher(JProperty item, out DesiredPublisher publisher)
        {
            try
            {
                publisher = item.Value.ToObject<DesiredPublisher>();
                return true;
            }
            catch(Exception ex)
            {
                publisher = default;
                var reportedPublisher = _jobDependencies.Module.Publishers.FirstOrDefault(i => i.Id == item.Name);
                if(reportedPublisher == default)
                {
                    _jobDependencies.Module.Publishers.Add(reportedPublisher = new Publisher()
                    {
                        Id = item.Name.Replace(Constants.TwinKeys.PublisherPrefix, string.Empty)
                    });
                }
                reportedPublisher.State = PublisherState.Error;
                reportedPublisher.ErrorContext = ex.Message;
                return false;
            }
        }
    }
}
