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

namespace Kotvis.Examples.Edge.Jobs
{
    public class HealthCheckJob : IJob
    {
        private readonly JobDependencyLocator _jobDependencies;
        private readonly ElapsedScheduleMessage _elapsedScheduleMessage;
        public HealthCheckJob(JobDependencyLocator jobDependencies, ElapsedScheduleMessage elapsedScheduleMessage)
        {
            _jobDependencies = jobDependencies;
            _elapsedScheduleMessage = elapsedScheduleMessage;
        }

        public async Task Run()
        {
            Console.Out.WriteLine($"Health check job was received for schedule id: {_elapsedScheduleMessage.ScheduleId}");

        }

    }
}
