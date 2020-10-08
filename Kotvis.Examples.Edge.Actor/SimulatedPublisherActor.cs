using Kotvis.Examples.Edge.Actor.Model;
using Kotvis.Examples.Edge.Model;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Azure.Devices.Client;
using System;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Kotvis.Examples.Edge.Models;

namespace Kotvis.Examples.Edge.Actor
{
    [Actor(TypeName = "SimulatedPublisherActor")]
    public class SimulatedPublisherActor : Dapr.Actors.Runtime.Actor, ISimulatedPublisherActor
    {
        public DeviceClient Client { get; set; }
        private ProvisioningTransportHandlerAmqp _transport;
        private SecurityProviderSymmetricKey _security;
        private string _idScope, _globalDeviceEndpoint, _primaryKey, _secondaryKey;

        public SimulatedPublisherActor(ActorService actorService, ActorId actorId, string idScope, string globalDeviceEndpoint, string primaryKey, string secondaryKey) 
            : base(actorService, actorId)
        {
            _idScope = idScope;
            _globalDeviceEndpoint = globalDeviceEndpoint;
            _primaryKey = primaryKey;
            _secondaryKey = secondaryKey;
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected async override Task OnActivateAsync()
        {
            // Provides opportunity to perform some optional setup.
            Console.WriteLine($"Activating actor id: {this.Id}");

            await base.OnActivateAsync();
        }

        private string ComputeDerivedSymmetricKey(byte[] masterKey, string registrationId)
        {
            using (var hmac = new HMACSHA256(masterKey))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationId)));
            }
        }

        /// <summary>
        /// This method is called whenever an actor is deactivated after a period of inactivity.
        /// </summary>
        protected async override Task OnDeactivateAsync()
        {
            // Provides Opporunity to perform optional cleanup.
            Console.WriteLine($"Deactivating actor id: {this.Id}");
            if (Client != default)
            {
                Client.Dispose();
            }
            if (_transport != default)
            {
                _transport.Dispose();
            }
            if (_security != default)
            {
                _security.Dispose();
            }
            await base.OnDeactivateAsync();
        }

        public async Task Connect()
        {
            //Group enrollment flow, the primary and secondary keys are derived from the enrollment group keys and from the desired registration id
            string primaryKey = ComputeDerivedSymmetricKey(Convert.FromBase64String(_primaryKey), this.Id.ToString());
            string secondaryKey = ComputeDerivedSymmetricKey(Convert.FromBase64String(_secondaryKey), this.Id.ToString());

            _security = new SecurityProviderSymmetricKey(this.Id.ToString(), primaryKey, secondaryKey);
            _transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);

            ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                _globalDeviceEndpoint,
                _idScope,
                _security,
                _transport);

            var result = await provClient.RegisterAsync();

            if (result.Status != ProvisioningRegistrationStatusType.Assigned)
            {
                throw new ApplicationException($"Provisioning status was not assigned, it was: {result}");
            }

            var auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, _security.GetPrimaryKey());
            Client = DeviceClient.Create(result.AssignedHub, "upboard", auth, TransportType.Amqp);
            await Client.OpenAsync();


            await Client.SetDesiredPropertyUpdateCallbackAsync(
                 new DesiredPropertyUpdateCallback((tc, o) =>
                 {
                     Console.WriteLine(tc.ToJson());
                     return Task.FromResult(false);
                 }), Client);

        }

        public async Task SendTelemetry(PublisherTelemetryMessage message)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            await Client.SendEventAsync(new Message(bytes));
        }
    }

}
