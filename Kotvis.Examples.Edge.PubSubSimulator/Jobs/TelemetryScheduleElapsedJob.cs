using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.PubSubSimulator.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.PubSubSimulator.Jobs
{
    public class TelemetryScheduleElapsedJob : IJob
    {
        private readonly ElapsedScheduleMessage _elapsedScheduleMessage;
        private readonly StateManager _stateManager;
        private readonly RestClient _client;
        private readonly CancellationToken _cancellationToken;
        public TelemetryScheduleElapsedJob(StateManager stateManager, ElapsedScheduleMessage elapsedScheduleMessage, CancellationToken cancellationToken)
        {
            _client = new RestClient();
            _elapsedScheduleMessage = elapsedScheduleMessage;
            _stateManager = stateManager;
            _cancellationToken = cancellationToken;
        }

        public async Task Run()
        {
            var subscriberRequest = _stateManager.GetRequest(_elapsedScheduleMessage.Context);

            RestRequest request = new RestRequest(new Uri($"http://{subscriberRequest.Request.SubscriberAddress}:{subscriberRequest.Request.SubscriberPort}/api/telemetries"));

            var subscriptionHeartbeat = new PublisherSubscriptionTelemetryMessage()
            {
                MessageTime = DateTimeOffset.UtcNow.AddMinutes(60),
                SubscriptionId = _elapsedScheduleMessage.Context,
                Pressure = (new Random()).Next(20, 45)
            };

            request.AddJsonBody(subscriptionHeartbeat);

            await _client.ExecutePostAsync(request, _cancellationToken);
        }
    }
}
