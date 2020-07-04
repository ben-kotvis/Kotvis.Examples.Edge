using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SchedulerModel = Kotvis.Edge.Scheduler.Model;

namespace Kotvis.Examples.Edge.Jobs
{
    public class HealthCheckJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly SchedulerModel.ElapsedScheduleMessage _elapsedScheduleMessage;
        public HealthCheckJob(JobDependencyLocator jobDependencies, SchedulerModel.ElapsedScheduleMessage elapsedScheduleMessage)
        {
            _jobDependencies = jobDependencies;
            _elapsedScheduleMessage = elapsedScheduleMessage;
        }

        public async Task Run()
        {
            Console.Out.WriteLine($"Health check job was received for subsccription id: {_elapsedScheduleMessage.CorrelationId}");

        }

    }
}
