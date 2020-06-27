using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class ElapsedScheduleMessage
    {
        /// <summary>
        /// The schedule id for the message
        /// </summary>
        public string ScheduleId { get; set; }
        
        /// <summary>
        /// The payload of the message
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// The job that should be executed when the schedule elapses
        /// </summary>
        public string JobName { get; set; }
    }
}
