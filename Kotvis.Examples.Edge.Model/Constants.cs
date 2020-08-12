using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class Constants
    {
        public class Scheduler
        {
            public const string ScheduleRequest = "schedule";
            public const string CancelRequest = "cancel";
        }

        public class PropertyNames
        {
            public const string RequestType = "request-type";
        }

        public class Inputs
        {
            public const string Schedule = "schedule";
            public const string SubscriberInbound = "subscriberinbound";
            public const string AddPublisher = "AddPublisher";
        }
        public class Outputs
        {
            public const string Scheduler = "scheduler";
            public const string Subscriber = "to-subscriber";
            public const string PubSubSimulator = "to-pubsubsimulator";
            public const string TelemetryUpstream = "to-upstream";
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
            public const string PubSubHeartbeatJob = "PubSubHeartbeatJob";
            public const string PubSubTelemetryJob = "PubSubTelemetryJob";
        }

        public class EnvironmentVariables
        {
            public const string SUBSCRIBER_ADDRESS = "SUBSCRIBER_ADDRESS";
        }
    }
}
