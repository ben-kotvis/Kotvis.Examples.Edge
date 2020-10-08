using Kotvis.Examples.Edge.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface IDeviceConnectionTracker 
    {
        Task Add(Publisher publisher);

        Task SendTelemetry(Publisher publisher, PublisherTelemetryMessage message);

        Task Remove(string publisherId);
    }
}
