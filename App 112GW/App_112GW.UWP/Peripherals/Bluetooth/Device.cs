using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        volatile public DeviceInformation Information;
        public event DeviceSetupComplete Ready;
        public event ChangeEvent Change;

        public string Id
        {
            get
            {
                return Information.Id;
            }
        }
        public string Name
        {
            get
            {
                return Information.Name;
            }
        }
        public bool Paired
        {
            get
            {
                return Information.Pairing.IsPaired;
            }
        }
        public bool CanPair
        {
            get
            {
                return Information.Pairing.CanPair;
            }
        }
        public UnPairedDeviceBLE(DeviceInformation pInput) { Information = pInput; }
        public override string ToString()
        {
            return Name + "\n" + Id;
        }

        public void Remake(object o)
        {
            throw new NotImplementedException();
        }

        public List<IServiceBLE> Services
        {
            get
            {
                return null;
            }
        }
    }
    public class PairedDeviceBLE : IDeviceBLE
    {
        private BluetoothLEDevice mDevice;
        private List<IServiceBLE> mServices;
        public event DeviceSetupComplete Ready;
        void TriggerReady()
        {
            mDevice.ConnectionStatusChanged += MDevice_ConnectionStatusChanged;
            AwaitingReconnect = false;
            Ready?.Invoke(this);
            Ready = null;
        }

        public event ChangeEvent Change;
        private void InvokeChange(object o, CharacteristicEvent v)
        {
            Change?.Invoke(o, v);
        }

        public string Id
        {
            get
            {
                return mDevice.DeviceId;
            }
        }
        public string Name
        {
            get
            {
                return mDevice.Name;
            }
        }
        public bool Paired
        {
            get
            {
                return mDevice.DeviceInformation.Pairing.IsPaired;
            }
        }
        public bool CanPair
        {
            get
            {
                return mDevice.DeviceInformation.Pairing.CanPair;
            }
        }

        private int Uninitialised = 0;
        private void ItemReady()
        {
            --Uninitialised;
            Debug.WriteLine("Service count remaining = " + Uninitialised.ToString());
            if (Uninitialised == 0)
                TriggerReady();
        }
        private async void Build()
        {
            await mDevice.GetGattServicesAsync().AsTask().ContinueWith((obj) =>
            {
                Debug.WriteLine("Found Services.");
                ServicesAquired(obj.Result);
            });
        }
        private void ServicesAquired(GattDeviceServicesResult result)
        {
            var services = result.Services;
            Uninitialised = services.Count;

            if (mServices != null)
                foreach (var service in mServices)
                    service.Unregister();

            mServices = null;
            mServices = new List<IServiceBLE>();

            foreach (var service in services)
                mServices.Add(new ServiceBLE(service, ItemReady, InvokeChange));
        }
        public override string ToString()
        {
            return Name + "\n" + Id;
        }
        public void Remake(object o)
        {
            Build();
        }

        bool AwaitingReconnect = false;
        void MDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            mDevice = sender;
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Connected && AwaitingReconnect)
            {
                Debug.WriteLine("Reconnecting...");
                BluetoothLEDevice.FromIdAsync(sender.DeviceId).AsTask().ContinueWith(Remake).Wait();
            }
            else
            {
                Debug.WriteLine("Disposing...");
                AwaitingReconnect = true;
            }
        }
        public List<IServiceBLE> Services
        {
            get
            {
                return mServices;
            }
        }
        public PairedDeviceBLE(BluetoothLEDevice pInput, DeviceSetupComplete pReady)
        {
            AwaitingReconnect = false;
            Ready = pReady;
            mDevice = pInput;
            Build();
        }
    }
}
