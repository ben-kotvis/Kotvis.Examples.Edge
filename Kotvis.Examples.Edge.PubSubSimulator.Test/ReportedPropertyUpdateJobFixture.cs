using Kotvis.Examples.Edge.Jobs;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace pubsubsimulator.test
{
    [TestClass]
    public class ReportedPropertyUpdateJobFixture
    {
        private Module _module;
        private JobDependencyLocator _jobDependencyLocator;
        private Dictionary<string, object> _reportedProperties;
        public ReportedPropertyUpdateJobFixture()
        {
            var edgeServiceMock = new Mock<IEdgeService>();
            edgeServiceMock
                .Setup(i => i.SetTwinReportedState(It.IsAny<Dictionary<string, object>>(), It.IsAny<CancellationToken>()))
                .Returns((Dictionary<string, object> d, CancellationToken token) =>
                {
                    _reportedProperties = d;
                    return Task.CompletedTask;
                });

            var schedulerServiceMock = new Mock<ISchedulerService>();

            var publisherApiServiceMock = new Mock<IPublisherApiService>();

            var tokenSource = new CancellationTokenSource();
            _module = new Module()
            {
                State = ModuleState.Online
            };

            _jobDependencyLocator = new JobDependencyLocator(edgeServiceMock.Object, schedulerServiceMock.Object, publisherApiServiceMock.Object, _module, tokenSource);

        }

        public ReportedPropertyUpdateJob CreateJob()
        {
            return new ReportedPropertyUpdateJob(_jobDependencyLocator);
        }

        [TestMethod]
        public async Task ReportedPropertiesAreSetWithNoPublishers()
        {
            var job = CreateJob();
            await job.Run();
            Assert.IsTrue(_reportedProperties.ContainsKey(Constants.TwinKeys.Module));

            var o = (Dictionary<string, object>)_reportedProperties[Constants.TwinKeys.Module];

            Assert.AreEqual(ModuleState.Online, (ModuleState)o[Constants.TwinKeys.ModuleState]);
        }

        [TestMethod]
        public async Task ReportedStateIsntSetIfNothingHasChanged()
        {
            _module.AcceptChanges();
            var job = CreateJob();
            await job.Run();
            Assert.IsNull(_reportedProperties);
        }

        [TestMethod]
        public async Task ReportedStateIsSetWithPublisherInformation()
        {
            var publisher = new Publisher
            {
                Host = "127.0.0.1",
                Id = "publisherId",
                Port = 8089,
                DesiredState = DesiredPublisherState.Online,
                ActualState = ActualPublisherState.Healthy                
            };

            _module.Publishers.Add(publisher);
            var job = CreateJob();
            await job.Run();
            Assert.IsNotNull(_reportedProperties);

            var o = (Dictionary<string, object>)_reportedProperties[Constants.TwinKeys.Module];

            var reportedPublisher = (Publisher)o[$"{Constants.TwinKeys.PublisherPrefix}{publisher.Id}"];

            Assert.AreEqual(publisher.Host, reportedPublisher.Host);
            Assert.AreEqual(publisher.Port, reportedPublisher.Port);
            Assert.AreEqual(publisher.DesiredState, reportedPublisher.DesiredState);
            Assert.AreEqual(publisher.ActualState, reportedPublisher.ActualState);
        }

    }
}
