namespace kotvisexamplesedgeactor
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Extensions.Hosting;
    using Dapr.Actors.AspNetCore;
    using Dapr.Actors.Runtime;
    using Kotvis.Examples.Edge.Actor;

    class Program
    {

        private const string KEY_ID_SCOPE = "ID_SCOPE";
        private const string KEY_GLOBAL_DEVICE_ENDPOINT = "GLOBAL_DEVICE_ENDPOINT";
        private const string KEY_PRIMARY_KEY = "PRIMARY_KEY";
        private const string KEY_SECONDARY_KEY = "SECONDARY_KEY";
        private const int AppChannelHttpPort = 3000;
        static void Main(string[] args)
        {
            Init().Wait();            

            CreateWebHostBuilder(args).Build().Run();
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
            InstallCACert();
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseActors(actorRuntime =>
                {
                    actorRuntime.RegisterActor<SimulatedPublisherActor>(info =>
                    {
                        return new ActorService(info, (service, id) =>
                        {
                            string idScope = Environment.GetEnvironmentVariable(KEY_ID_SCOPE);
                            string globalDeviceEndpoint = Environment.GetEnvironmentVariable(KEY_GLOBAL_DEVICE_ENDPOINT);
                            string primaryKey = Environment.GetEnvironmentVariable(KEY_PRIMARY_KEY);
                            string secondaryKey = Environment.GetEnvironmentVariable(KEY_SECONDARY_KEY);

                            return new SimulatedPublisherActor(service, id, idScope, globalDeviceEndpoint, primaryKey, secondaryKey);
                        });
                    });
                }
                ).UseUrls($"http://localhost:{AppChannelHttpPort}/");

        static void InstallCACert()
        {
            string trustedCACertPath = "azure-iot-test-only.root.ca.cert.pem";
            if (!string.IsNullOrWhiteSpace(trustedCACertPath))
            {
                Console.WriteLine("User configured CA certificate path: {0}", trustedCACertPath);
                if (!File.Exists(trustedCACertPath))
                {
                    // cannot proceed further without a proper cert file
                    Console.WriteLine("Certificate file not found: {0}", trustedCACertPath);
                    throw new InvalidOperationException("Invalid certificate file.");
                }
                else
                {
                    Console.WriteLine("Attempting to install CA certificate: {0}", trustedCACertPath);
                    X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(new X509Certificate2(X509Certificate.CreateFromCertFile(trustedCACertPath)));
                    Console.WriteLine("Successfully added certificate: {0}", trustedCACertPath);
                    store.Close();
                }
            }
            else
            {
                Console.WriteLine("CA_CERTIFICATE_PATH was not set or null, not installing any CA certificate");
            }
        }


    }
}
