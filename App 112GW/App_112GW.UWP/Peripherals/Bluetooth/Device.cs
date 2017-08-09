using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
        private void Build()
        {
            mDevice.GetGattServicesAsync().AsTask().ContinueWith((obj) =>
            {
                Debug.WriteLine("Found Services.");
                ServicesAquired(obj.Result);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        private void ServicesAquired(GattDeviceServicesResult result)
        {
            var services = result.Services;
            Uninitialised = services.Count;

            Deregister();

            foreach (var service in services)
                mServices.Add(new ServiceBLE(service, ItemReady, InvokeChange));
        }
        public override string ToString()
        {
            return Name + "\n" + Id;
        }
        public void Remake(object o)
        {
            mDevice.ConnectionStatusChanged -= MDevice_ConnectionStatusChanged;
            Build();
        }

        void Deregister()
        {
            if (mServices != null)
                foreach (var service in mServices)
                    service.Unregister();

            mServices = null;
            mServices = new List<IServiceBLE>();
        }

        CancellationTokenSource canceller_source = new CancellationTokenSource();
        void MDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            mDevice = sender;
            Deregister();

            if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                Debug.WriteLine("Reconnecting...");
                //BluetoothLEDevice.FromIdAsync(sender.DeviceId).AsTask().ContinueWith((obj)=> { ConnectionComplete(obj.Result); });
            }
            else
                ConnectionComplete(sender);
            Debug.WriteLine("MDevice_ConnectionStatusChanged end.");
        }
        void ConnectionComplete(BluetoothLEDevice result)
        {
            Debug.WriteLine("Connection complete start.");
            mDevice = result;
            Remake(result);
            Debug.WriteLine("Connection complete end.");
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
            Ready = pReady;
            mDevice = pInput;
            Build();
        }
    }
}
