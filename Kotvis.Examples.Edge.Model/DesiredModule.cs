using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class DesiredModule
    {
        public DesiredModule()
        {
            Publishers = new List<DesiredPublisher>();
        }
        public ModuleState State { get; set; }
        public List<DesiredPublisher> Publishers { get; set; }
    }
}
