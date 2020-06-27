using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class Constants
    {
        public class Inputs
        {
            public const string Schedule = "schedule";
            public const string SubscriberInbound = "subscriberinbound";
        }
        public class Outputs
        {
            public const string Scheduler = "scheduler";
        }

        public class TwinKeys
        {
            public const string Module = "Module";
            public const string ModuleState = "State";
            public const string PublisherPrefix = "Publisher-";
        }

        public class JobNames
        {
            public const string DesiredPropertyChanged = "DesiredPropertyChanged";
            public const string HealthCheck = "HealthCheck";
        }
    }
}
