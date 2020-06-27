using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Jobs;
using Newtonsoft.Json;

namespace Kotvis.Examples.Edge.Subscriber.Services
{
    /// <summary>
    /// This is just a layer to help seperate the IoT specific libraries from the rest for testing
    /// </summary>
    internal class InitializerService
    {
        private readonly ModuleClient _moduleClient;
        private readonly CancellationTokenSource _tokenSource;
        private readonly JobDependencyLocator _jobDependencyLocator;

        public InitializerService(ModuleClient moduleClient, JobDependencyLocator jobDependencyLocator, CancellationTokenSource tokenSource)
        {
            _moduleClient = moduleClient;
            _jobDependencyLocator = jobDependencyLocator;
            _tokenSource = tokenSource;
        }

        public async Task Initialize(IApplicationBuilder applicationBuilder)
        {
            await _moduleClient.SetInputMessageHandlerAsync(Constants.Inputs.SubscriberInbound,
                new MessageHandler(async (message, userContext) =>
                    {
                        var moduleClient = userContext as ModuleClient;
                        if (moduleClient == null)
                        {
                            throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
                        }

                        byte[] messageBytes = message.GetBytes();
                        string messageString = Encoding.UTF8.GetString(messageBytes);

                        var healthJob = new HealthCheckJob(_jobDependencyLocator, JsonConvert.DeserializeObject<ElapsedScheduleMessage>(messageString));
                        await healthJob.Run();

                        await moduleClient.CompleteAsync(message);
                        return MessageResponse.Completed;
                    }), _moduleClient, _tokenSource.Token);

            await _moduleClient.SetDesiredPropertyUpdateCallbackAsync(
                new DesiredPropertyUpdateCallback(async (mr, o) =>
                {
                    var job = new DesiredPropertyChangedJob(_jobDependencyLocator, mr);
                    await job.Run();
                }), _moduleClient);

            var twin = await _moduleClient.GetTwinAsync();
            var spinupJob = new DesiredPropertyChangedJob(_jobDependencyLocator, twin.Properties.Desired);
            await spinupJob.Run();
        }

    }
}
