namespace scheduler
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Kotvis.Examples.Edge.Model;
    using Kotvis.Examples.Edge.Scheduler;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Newtonsoft.Json;
    using Scheduler = Kotvis.Examples.Edge.Scheduler;

    class Program
    {
        //static StateManager stateManager;
        static CancellationTokenSource cts;
        static Scheduler.Module module;
        static Task scheduleTask;
        static void Main(string[] args)
        {

            cts = new CancellationTokenSource();
            Init().Wait();

            // Wait until the app unloads or is cancelled
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s =>
            {
                scheduleTask.Dispose();
                ((TaskCompletionSource<bool>)s).SetResult(true);
            }, tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init()
        {
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");


            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync(Constants.Inputs.Schedule, PipeMessage, ioTHubModuleClient, cts.Token);

            module = new Scheduler.Module();

            scheduleTask = MessageScheduler.Create(module, ioTHubModuleClient, cts.Token);
        }

        static Task<MessageResponse> PipeMessage(Message message, object userContext)
        {   
            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);

            var requestType = message.Properties[Constants.PropertyNames.RequestType];

            switch (requestType)
            {
                case Constants.Scheduler.ScheduleRequest:
                    var request = JsonConvert.DeserializeObject<SchedulerRequest>(messageString);
                    Monitor.Enter(module);
                    module.Schedules.Add(new Schedule(request));
                    Monitor.Exit(module);
                    break;
                case Constants.Scheduler.CancelRequest:
                    var cancelRequest = JsonConvert.DeserializeObject<SchedulerCancelRequest>(messageString);
                    Monitor.Enter(module);
                    module.Schedules.RemoveAll(i => i.Request.Context.ScheduleId == cancelRequest.ScheduleId); 
                    Monitor.Exit(module);
                    break;
            };
            return Task.FromResult(MessageResponse.Completed);
        }
    }
}
