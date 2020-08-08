using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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
            if (!_twinCollection.Contains(Constants.TwinKeys.Module))
            {
                await Console.Out.WriteLineAsync("Module not defined");
                return;
            }
            JObject module = _twinCollection[Constants.TwinKeys.Module];
            _jobDependencies.Module.State = (ModuleState)module.SelectToken(Constants.TwinKeys.ModuleState).Value<Int32>();

            var desiredPublisherIds = new List<string>();

            foreach (JProperty item in module.Children().Where(i => i.Path.Contains(Constants.TwinKeys.PublisherPrefix)))
            {
                desiredPublisherIds.Add(item.Name.Replace(Constants.TwinKeys.PublisherPrefix, string.Empty));
                var publisher = ReconcilePublisher(item);
            }

            foreach (var publisher in _jobDependencies.Module.Publishers.Where(i => !desiredPublisherIds.Contains(i.Id)))
            {
                publisher.DesiredState = DesiredPublisherState.Removed;
            }

            var routingJob = new RoutingJob(_jobDependencies);
            await routingJob.Run();

            var reportedJob = new ReportedPropertyUpdateJob(_jobDependencies);
            await reportedJob.Run();
        }

        private Publisher ReconcilePublisher(JProperty item)
        {
            var publisherId = item.Name.Replace(Constants.TwinKeys.PublisherPrefix, string.Empty);
            var modulePublisher = _jobDependencies.Module.Publishers.FirstOrDefault(i => i.Id == publisherId);

            if(modulePublisher == default)
            {
                modulePublisher = new Publisher() { Id = publisherId };
                _jobDependencies.Module.Publishers.Add(modulePublisher);
            }
            try
            {
                var publisher = item.Value.ToObject<Publisher>();
                modulePublisher.DesiredState = publisher.DesiredState;
                modulePublisher.Host = publisher.Host;
                modulePublisher.Port = publisher.Port;
                modulePublisher.UserName = publisher.UserName;
                modulePublisher.Password = publisher.Password;
            }
            catch(Exception ex)
            {
                modulePublisher.ActualState = ActualPublisherState.Error;
                modulePublisher.ErrorContext = ex.Message;
            }

            return modulePublisher;
        }
    }
}
