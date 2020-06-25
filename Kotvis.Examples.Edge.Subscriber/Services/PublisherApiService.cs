using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    public class PublisherApiService : IPublisherApiService
    {
        private readonly RestClient _client;

        public PublisherApiService()
        {
            _client = new RestClient();
        }

        public async Task CancelSubscription(Publisher publisher, CancellationToken cancellationToken)
        {
            var request = new RestRequest(new Uri($"http://{publisher.Host}:{publisher.Port}/api/cancelsubscriptions"), Method.POST);
            var requestObject = new
            {
                SubscriptionId = publisher.SubscriptionId
            };

            request.AddJsonBody(requestObject);

            await _client.ExecuteAsync(request);
        }

        public Task Restart(Publisher publisher, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Subscribe(Publisher publisher, CancellationToken cancellationToken)
        {
            var request = new RestRequest(new Uri($"http://{publisher.Host}:{publisher.Port}/api/subscribes"), Method.POST);
            var requestObject = new
            {
                SubscriberAddress = "127.0.0.1",
                SubscriberPort = 8081,
                Username = "admin",
                Password = "admin"
            };

            request.AddJsonBody(requestObject);

            var response = await _client.ExecuteAsync<SubscribeResponse>(request);

            return response.Data.SubscriptionId;
        }
    }
}
