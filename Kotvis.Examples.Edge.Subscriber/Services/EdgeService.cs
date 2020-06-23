using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    internal class EdgeService : IEdgeService
    {
        public Task SendMessageToOutput(string outputName, string output, CancellationToken cancellationToken, params NameValue[] properties)
        {
            throw new NotImplementedException();
        }

        public Task SetTwinReportedState(Dictionary<string, object> twinDictionary, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
