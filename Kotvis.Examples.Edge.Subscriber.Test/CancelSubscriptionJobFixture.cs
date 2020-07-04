using Kotvis.Examples.Edge.Jobs;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace pubsubsimulator.test
{
    [TestClass]
    public class CancelSubscriptionJobFixture
    {
        private Module _module;
        private JobDependencyLocator _jobDependencyLocator;
        public CancelSubscriptionJobFixture()
        {
            var edgeServiceMock = new Mock<IEdgeService>();

            var schedulerServiceMock = new Mock<ISchedulerService>();

            var publisherApiServiceMock = new Mock<IPublisherApiService>();
            publisherApiServiceMock
                .Setup(i => i.Subscribe(It.IsAny<Publisher>(), It.IsAny<CancellationToken>()))
                .Returns((Publisher p, CancellationToken token) =>
                {
                    return Task.FromResult(p.Id);
                });


            var tokenSource = new CancellationTokenSource();
            _module = new Module();
            _jobDependencyLocator = new JobDependencyLocator(edgeServiceMock.Object, schedulerServiceMock.Object, publisherApiServiceMock.Object, _module, tokenSource);

        }

        public CancelSubscriptionJob CreateJob(Publisher publisher)
        {
            return new CancelSubscriptionJob(_jobDependencyLocator, publisher);
        }

        private Publisher CreatePublisher()
        {
            return new Publisher()
            {
                Id = "subscribePublisherId",
                ActualState = ActualPublisherState.Unknown                
            };
        }

        [TestMethod]
        public async Task PublisherStateIsSubscribedAfterJobIsRun()
        {
            var publisher = CreatePublisher();
            _module.Publishers.Add(publisher);
            var job = CreateJob(publisher);
            await job.Run();
            Assert.AreEqual(ActualPublisherState.StandingBy, publisher.ActualState);
        }
    }
}
