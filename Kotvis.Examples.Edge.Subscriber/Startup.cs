using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using Kotvis.Examples.Edge.Subscriber;
using Kotvis.Examples.Edge.Subscriber.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace subscriber
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
     
        public void ConfigureServices(IServiceCollection services)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);

            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = ModuleClient.CreateFromEnvironmentAsync(settings).GetAwaiter().GetResult();
            ioTHubModuleClient.OpenAsync().GetAwaiter().GetResult();
            
            services.AddSingleton<ModuleClient>(ioTHubModuleClient);

            services.AddSingleton<IDeviceConnectionTracker, DeviceConnectionTracker>();
            services.AddSingleton<IEdgeService, EdgeService>();
            services.AddSingleton<IPublisherApiService>(sp => new PublisherApiService(_configuration.GetValue<string>(Constants.EnvironmentVariables.SUBSCRIBER_ADDRESS)));
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<Module>(new Module());
            services.AddSingleton<CancellationTokenSource>();
            services.AddSingleton<JobDependencyLocator>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           

            var initializer = new InitializerService(
                app.ApplicationServices.GetService<ModuleClient>(),
                app.ApplicationServices.GetService<JobDependencyLocator>(),
                app.ApplicationServices.GetService<CancellationTokenSource>());
            initializer.Initialize(app).GetAwaiter().GetResult();
        }
    }
}
