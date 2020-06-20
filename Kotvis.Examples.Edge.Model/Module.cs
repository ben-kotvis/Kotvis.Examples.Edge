using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class Module
    {
        public Module()
        {
            Publishers = new List<Publisher>();
        }
        public ModuleState State { get; set; }
        public List<Publisher> Publishers { get; set; }
    }
}
