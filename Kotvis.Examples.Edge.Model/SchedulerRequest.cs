using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class SchedulerRequest
    {
        public bool Repeat { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string Context { get; set; }
        public string JobName { get; set; }
    }
}
