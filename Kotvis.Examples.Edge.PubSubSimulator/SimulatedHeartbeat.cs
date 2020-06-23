using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator
{
    public class SimulatedHeartbeat
    {
        private readonly RestClient _client;
        public SimulatedHeartbeat()
        {
            _client = new RestClient();
        }
        public async Task Create(string subscriberHost, string id, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000);
                RestRequest request = new RestRequest(new Uri(subscriberHost));
                request.AddJsonBody(new { Id = id });
                await _client.ExecutePostAsync(request, cancellationToken);
            }
        }
    }
}
