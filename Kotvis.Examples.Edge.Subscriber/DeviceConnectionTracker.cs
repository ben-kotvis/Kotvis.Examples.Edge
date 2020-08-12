using Kotvis.Examples.Edge.Model;
using Kotvis.Examples.Edge.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Kotvis.Examples.Edge.Subscriber
{
    internal class DeviceConnectionTracker : IDeviceConnectionTracker
    {
        private string _idScope = "0ne0014F6F4";
        private string _globalDeviceEndpoint = "global.azure-devices-provisioning.net";
        private string _primaryKey = "1Bo/GDGQinQPsjpI54gz3pqz3dp8Pi9k2xZGyLbHCjVw0Sfs5eBuBFsSx52iZwtpSvXzyyRDMIQeAuZpkIw2HA==";
        private string _secondaryKey = "el3YnlfCRBH6megpSZA2Yg5OKFMqDZMD25L8B4pOdqfPryzF8B7FIKeTr53wONmMLHY5ny0CQCewynHF1W/oug==";

        private Dictionary<string, DeviceConnnectionComposite> Clients;
        public DeviceConnectionTracker()
        {
            Clients = new Dictionary<string,DeviceConnnectionComposite>();
            InstallCACert();
        }
        public async Task Add(Publisher publisher)
        {
            var deviceConnectionInfo = new DeviceConnectionCompositeDependencies(publisher.Id, _primaryKey, _secondaryKey, _globalDeviceEndpoint, _idScope);
            var connectionComposite = new DeviceConnnectionComposite(deviceConnectionInfo);
            await connectionComposite.Initialize();

            Clients.Add(publisher.Id, connectionComposite);
        }
        public async Task SendTelemetry(Publisher publisher, object message)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var clientComposite = Clients[publisher.Id];

            await clientComposite.Client.SendEventAsync(new Message(bytes));
        }

        public Task CheckHealth()
        {
            throw new NotImplementedException();
        }

        public Task Remove(string publisherId)
        {
            throw new NotImplementedException();
        }

        private void InstallCACert()
        {
            //string trustedCACertPath = "/home/aibox/certs/working/certs/azure-iot-test-only.root.ca.cert.pem";
            string trustedCACertPath = "/democerts/azure-iot-test-only.root.ca.cert.pem";
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

        public void Dispose()
        {
            foreach(var item in Clients)
            {
                if(item.Value != default)
                {
                    item.Value.Dispose();
                }
            }
        }
    }

    public class DeviceConnectionCompositeDependencies
    {
        public string RegistrationId { get; }
        public string PrimaryKey { get; }
        public string SecondaryKey { get; }
        public string GlobalDeviceEndpoint { get; }
        public string IdScope { get; }

        public DeviceConnectionCompositeDependencies(
            string registrationId,
            string primaryKey,
            string secondaryKey,
            string globalDeviceEndpoint,
            string idScope)
        {
            RegistrationId = registrationId;
            PrimaryKey = primaryKey;
            SecondaryKey = secondaryKey;
            GlobalDeviceEndpoint = globalDeviceEndpoint;
            IdScope = idScope;
        }
    }

    public class DeviceConnnectionComposite : IDisposable
    {
        public DeviceClient Client { get; set; }

        private DeviceConnectionCompositeDependencies _deviceConnectionCompositeDependencies;
        private ProvisioningTransportHandlerAmqp _transport;
        private SecurityProviderSymmetricKey _security;
        public DeviceConnnectionComposite(DeviceConnectionCompositeDependencies deviceConnectionCompositeDependencies)
        {
            _deviceConnectionCompositeDependencies = deviceConnectionCompositeDependencies;
        }

        public async Task Initialize()
        {
            //Group enrollment flow, the primary and secondary keys are derived from the enrollment group keys and from the desired registration id
            string primaryKey = ComputeDerivedSymmetricKey(Convert.FromBase64String(_deviceConnectionCompositeDependencies.PrimaryKey), _deviceConnectionCompositeDependencies.RegistrationId);
            string secondaryKey = ComputeDerivedSymmetricKey(Convert.FromBase64String(_deviceConnectionCompositeDependencies.SecondaryKey), _deviceConnectionCompositeDependencies.RegistrationId);

            _security = new SecurityProviderSymmetricKey(_deviceConnectionCompositeDependencies.RegistrationId, primaryKey, secondaryKey);
            _transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);

            ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                _deviceConnectionCompositeDependencies.GlobalDeviceEndpoint,
                _deviceConnectionCompositeDependencies.IdScope,
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
                     Console.WriteLine($"Device Desired Callback: {_deviceConnectionCompositeDependencies.RegistrationId}");
                     Console.WriteLine(tc.ToJson());
                     return Task.FromResult(false);
                 }), Client);

        }

        private string ComputeDerivedSymmetricKey(byte[] masterKey, string registrationId)
        {
            using (var hmac = new HMACSHA256(masterKey))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(registrationId)));
            }
        }

        public void Dispose()
        {
            if(Client != default)
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
        }
    }
}
