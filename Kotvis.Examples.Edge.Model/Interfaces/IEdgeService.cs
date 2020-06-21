using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface IEdgeService
    {
        Task SendMessageToOutput(string outputName, string output, CancellationToken cancellationToken, params NameValue[] properties);
        Task SetTwinReportedState(Dictionary<string, object> twinDictionary, CancellationToken cancellationToken);
    }
}
