using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class JobDependencies
    {
        private readonly IEdgeService _edgeService;
        private readonly ISchedulerService _schedulerService;
        private readonly Module _module;
        private readonly DesiredModule _desiredModule;

        public JobDependencies(IEdgeService edgeService, ISchedulerService schedulerService, Module module, DesiredModule desiredModule)
        {
            _edgeService = edgeService;
            _schedulerService = schedulerService;
            _module = module;
            _desiredModule = desiredModule;
        }

        public IEdgeService EdgeService => _edgeService;
        public ISchedulerService SchedulerService => _schedulerService;
        public Module Module => _module;
        public DesiredModule DesiredModule => _desiredModule;
    }
}
