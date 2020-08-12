using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface IDeviceConnectionTracker : IDisposable
    {
        Task Add(Publisher publisher);

        Task SendTelemetry(Publisher publisher, object message);

        Task CheckHealth();

        Task Remove(string publisherId);
    }
}
