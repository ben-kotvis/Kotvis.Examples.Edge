using System;
using System.Collections.Generic;
using System.Text;

namespace Kotvis.Examples.Edge.Model
{
    public enum ActualPublisherState
    {   
        Unknown,
        Error,
        Subscribed,
        Healthy,
        StandingBy,
        Removed
    }
}
