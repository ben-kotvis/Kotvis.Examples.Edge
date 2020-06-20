using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class DesiredPublisher
    {
        public string Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public PublisherState State { get; set; }
    }
}
