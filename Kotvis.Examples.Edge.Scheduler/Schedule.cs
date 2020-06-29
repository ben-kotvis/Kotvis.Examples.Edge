using Kotvis.Examples.Edge.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Scheduler
{
    public class Schedule
    {
        public Schedule(SchedulerRequest request)
        {
            Request = request;
            NextRunTime = DateTimeOffset.Now.Add(Request.RunTime);
        }
        public SchedulerRequest Request { get; }
        public DateTimeOffset NextRunTime { get; private set; }
        public DateTimeOffset LastRunTime { get; private set; }

        public void Advance(DateTimeOffset actualLastRuntime)
        {
            LastRunTime = actualLastRuntime;
            NextRunTime = actualLastRuntime.Add(Request.RunTime);
        }
    }
}
