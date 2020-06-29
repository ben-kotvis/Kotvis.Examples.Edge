using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Scheduler
{
    public class Module
    {
        public Module()
        {
            Schedules = new List<Schedule>();
        }

        public List<Schedule> Schedules { get; }
    }
}
