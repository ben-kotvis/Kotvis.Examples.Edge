using Kotvis.Examples.Edge.Jobs;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Microsoft.Azure.Devices.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace pubsubsimulator.test
{
    [TestClass]
    public class DesiredPropertyChangedJobFixture
    {
        private Module _module;
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

            _module = new Module();
            var dependencies = new JobDependencies(edgeServiceMock.Object, schedulerServiceMock.Object, _module);

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
            var twin = new TwinCollection();
            twin[Constants.TwinKeys.Module] = new Module()
            {
                State = ModuleState.Online
            };

            var job = CreateJob(twin);
            await job.Run();

            Assert.AreEqual(ModuleState.Offline, _module.State);
        }
    }
}
