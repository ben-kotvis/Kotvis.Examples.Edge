using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface IEdgeService
    {
        Task SendMessageToOutput(string outputName, string output, params NameValue[] properties);
    }
}
