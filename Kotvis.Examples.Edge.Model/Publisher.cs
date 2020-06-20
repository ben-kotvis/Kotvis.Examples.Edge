using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public class Publisher : DesiredPublisher
    {
        public DateTimeOffset LastMessageTime { get; set; }
        public string ErrorContext { get; set; }
    }
}
