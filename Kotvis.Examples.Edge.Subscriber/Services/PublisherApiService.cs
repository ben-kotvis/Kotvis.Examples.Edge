using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    public class PublisherApiService : IPublisherApiService
    {
        public Task CancelSubscription(Publisher publisher, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Restart(Publisher publisher, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> Subscribe(Publisher publisher, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
