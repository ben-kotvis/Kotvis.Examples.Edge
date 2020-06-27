using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class SchedulerRequest
    {
        /// <summary>
        /// Should the schedule run once or over and over again
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// When the schedule should be run
        /// </summary>
        public TimeSpan RunTime { get; set; }

        /// <summary>
        /// The payload to be part of the message
        /// </summary>
        public ElapsedScheduleMessage Context { get; set; }
        
        /// <summary>
        /// Use this for routing back to the inbound messages to your module
        /// </summary>
        public string OutputName { get; set; }
    }
}
