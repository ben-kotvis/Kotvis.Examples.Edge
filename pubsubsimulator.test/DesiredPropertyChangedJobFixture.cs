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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace pubsubsimulator.test
{
    [TestClass]
    public class DesiredPropertyChangedJobFixture
    {
        private Module _module;
        private DesiredModule _desiredModule;
        public DesiredPropertyChangedJob CreateJob(TwinCollection twinCollection)
        {
            var edgeServiceMock = new Mock<IEdgeService>();
            edgeServiceMock
                .Setup(i => i.SendMessageToOutput(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NameValue[]>()))
                .Returns((string on, string o, NameValue[] nvs) =>
                {
                    return Task.CompletedTask;
                });

            var schedulerServiceMock = new Mock<ISchedulerService>();

            _desiredModule = new DesiredModule();
            _module = new Module();
            var dependencies = new JobDependencyLocator(edgeServiceMock.Object, schedulerServiceMock.Object, _module, _desiredModule);

            return new DesiredPropertyChangedJob(dependencies, twinCollection);
        }

        [TestMethod]
        public async Task UnpopulatedTwinDoesNotThrowError()
        {
            var job = CreateJob(new TwinCollection());
            await job.Run();
        }

        [TestMethod]
        public async Task ModuleStateIsSetBasedOnState()
        {
            var desired = new Dictionary<string, object>();
            desired.Add(Constants.TwinKeys.ModuleState, ModuleState.Online);

            var twin = new TwinCollection();
            twin[Constants.TwinKeys.Module] = desired;

            var job = CreateJob(twin);
            await job.Run();

            Assert.AreEqual(ModuleState.Online, _desiredModule.State);
            Assert.AreEqual(ModuleState.Offline, _module.State);
        }

        [TestMethod]
        public async Task ModuleStatePublishersIsSetBasedOnDesiredTwin()
        {
            var desired = new Dictionary<string, object>();
            desired.Add(Constants.TwinKeys.ModuleState, ModuleState.Online);

            var desiredPublisher = new DesiredPublisher()
            {
                Host = "127.0.0.1",
                Id = "publisherId",
                Port = 8089,
                State = PublisherState.Healthy
            };

            desired.Add($"{Constants.TwinKeys.PublisherPrefix}{desiredPublisher.Id}", desiredPublisher);

            var twin = new TwinCollection();
            twin[Constants.TwinKeys.Module] = desired;
            
            var job = CreateJob(twin);
            await job.Run();

            Assert.AreEqual(desiredPublisher.Host, _desiredModule.Publishers.First(i => i.Id == desiredPublisher.Id).Host);
        }

        [TestMethod]
        public async Task DesiredPublisherErrorIsOnState()
        {
            var desired = new Dictionary<string, object>();
            desired.Add(Constants.TwinKeys.ModuleState, ModuleState.Online);

            var desiredPublisher = new 
            {
                Host = "127.0.0.1",
                Id = "publisherId",
                Port = "not a port",
                State = PublisherState.Healthy
            };

            desired.Add($"{Constants.TwinKeys.PublisherPrefix}{desiredPublisher.Id}", desiredPublisher);

            var twin = new TwinCollection();
            twin[Constants.TwinKeys.Module] = desired;

            var job = CreateJob(twin);
            await job.Run();

            Assert.AreEqual(PublisherState.Error, _module.Publishers.First(i => i.Id == desiredPublisher.Id).State);
            Assert.AreEqual("Could not convert string to integer: not a port. Path 'Module.Publisher-publisherId.Port'.", _module.Publishers.First(i => i.Id == desiredPublisher.Id).ErrorContext);
        }

        [TestMethod]
        public async Task TwinJsonSerializesIntoTwinCollection()
        {
            var filename = "./Resources/desired-properties.json";
            var fileText = File.ReadAllText(filename);
            var twin = new TwinCollection(fileText);
            var job = CreateJob(twin);
            await job.Run();

            Assert.IsTrue(_desiredModule.Publishers.Any());
            var publisher = _desiredModule.Publishers.First();

            Assert.AreEqual("127.0.0.2", publisher.Host);
            Assert.AreEqual(8088, publisher.Port);
            Assert.AreEqual(PublisherState.Unknown, publisher.State);
            Assert.AreEqual("filePublisherId", publisher.Id);
        }

        [TestMethod]
        public async Task TwinJsonSerializesIntoTwinCollectionTwoPublishers()
        {
            var filename = "./Resources/desired-properties-two-publishers.json";
            var fileText = File.ReadAllText(filename);
            var twin = new TwinCollection(fileText);
            var job = CreateJob(twin);
            await job.Run();

            Assert.IsTrue(_desiredModule.Publishers.Count == 2);
            var firstPublisher = _desiredModule.Publishers.First();

            Assert.AreEqual("127.0.0.2", firstPublisher.Host);
            Assert.AreEqual(8088, firstPublisher.Port);
            Assert.AreEqual(PublisherState.Unknown, firstPublisher.State);
            Assert.AreEqual("filePublisherId", firstPublisher.Id);

            var secondPublisher = _desiredModule.Publishers.Last();

            Assert.AreEqual("127.0.0.3", secondPublisher.Host);
            Assert.AreEqual(8087, secondPublisher.Port);
            Assert.AreEqual(PublisherState.Healthy, secondPublisher.State);
            Assert.AreEqual("filePublisherIdTwo", secondPublisher.Id);
        }        
    }
}
