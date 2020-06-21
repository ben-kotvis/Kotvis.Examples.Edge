using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kotvis.Examples.Edge.Model.Interfaces
{
    public interface IPublisherApiService
    {
        Task<string> Subscribe(Publisher publisher, CancellationToken cancellationToken);
        Task CancelSubscription(Publisher publisher, CancellationToken cancellationToken);
        Task Restart(Publisher publisher, CancellationToken cancellationToken);
    }
}
