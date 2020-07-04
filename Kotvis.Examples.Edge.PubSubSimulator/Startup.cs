using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.PubSubSimulator;
using Kotvis.Examples.Edge.PubSubSimulator.Jobs;
using Kotvis.Examples.Edge.PubSubSimulator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SchedulerModel = Kotvis.Edge.Scheduler.Model;

namespace pubsubsimulator
{
    public class Startup
    { 
        public void ConfigureServices(IServiceCollection services)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);

            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = ModuleClient.CreateFromEnvironmentAsync(settings).GetAwaiter().GetResult();
            ioTHubModuleClient.OpenAsync().GetAwaiter().GetResult();

            services.AddSingleton<ModuleClient>(ioTHubModuleClient);

            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<StateManager>();
            services.AddSingleton<CancellationTokenSource>(tokenSource);
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var moduleClient = app.ApplicationServices.GetService<ModuleClient>();
            var stateManager = app.ApplicationServices.GetService<StateManager>();
            var tokenSource = app.ApplicationServices.GetService<CancellationTokenSource>();
            moduleClient.SetInputMessageHandlerAsync(Constants.Inputs.SubscriberInbound,
                new MessageHandler(async (m, o) =>
                {
                    byte[] messageBytes = m.GetBytes();
                    string messageString = Encoding.UTF8.GetString(messageBytes);

                    var request = JsonConvert.DeserializeObject<SchedulerModel.ElapsedScheduleMessage>(messageString);
                    var job = new HeartbeatScheduleElapsedJob(stateManager, request, tokenSource.Token);
                    await job.Run();
                    return MessageResponse.Completed;

                }), moduleClient);
        }        
    }
}
