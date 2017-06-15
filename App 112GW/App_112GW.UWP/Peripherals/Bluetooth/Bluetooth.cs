using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Core;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Security.Cryptography;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        public DeviceInformation Information;

        private string _id;
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        private bool _paired;
        public bool paired
        {
            get
            {
                return _paired;
            }
            set
            {
                _paired = value;
            }
        }
        private bool _CanPair;
        public bool CanPair
        {
            get
            {
                return _CanPair;
            }
            set
            {
                _CanPair = value;
            }
        }
        public UnPairedDeviceBLE(){}

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
        private bool mSuccess;
        public BluetoothLEDevice mDevice;
        private List<IServiceBLE> mServices;

        public string   id
        {
            get
            {
                return mDevice.DeviceId;
            }
        }
        public string   name
        {
            get
            {
                return mDevice.Name;
            }
        }
        public bool     paired
        {
            get
            {
                return mDevice.DeviceInformation.Pairing.IsPaired;
            }
        }
        public bool     CanPair
        {
            get
            {
                return mDevice.DeviceInformation.Pairing.CanPair;
            }
        }

        private async Task<bool> Build()
        {
            var servs = (await mDevice.GetGattServicesAsync()).Services;

            mServices.Clear();
            foreach (var service in servs)
                mServices.Add(new ServiceBLE(service));

            return true;
        }
        public PairedDeviceBLE(BluetoothLEDevice pInput)
        {
            mDevice = pInput;
            mServices = new List<IServiceBLE>();
            Task.Run(async () =>
            {
                mSuccess = await Build();
            }).Wait();
        }
        public List<IServiceBLE> Services
        {
            get
            {
                return mServices;
            }
        }
    }
    public class ServiceBLE : IServiceBLE
    {
        private bool mSuccess;
        private GattDeviceService           mService;
        public string id
        {
            get
            {
                return mService.Uuid.ToString();
            }
        }
        public override string ToString()
        {
            return id;
        }
        private async Task<bool> Build()
        {
            var items = (await mService.GetCharacteristicsAsync()).Characteristics;
            mCharacteristics.Clear();
            foreach (var item in items)
                mCharacteristics.Add(new CharacteristicBLE(item));

            return true;
        }
        public ServiceBLE(GattDeviceService pInput)
        {
            mService = pInput;
            mCharacteristics = new List<ICharacteristicBLE>();
            Task.Run(async () =>
            {
                mSuccess = await Build();
            }).Wait();
        }
        private List<ICharacteristicBLE> mCharacteristics;
        public List<ICharacteristicBLE> Characteristics
        {
            get
            {
                return mCharacteristics;
            }
        }
    }
    public class CharacteristicBLE : ICharacteristicBLE
    {
        private GattCharacteristic  mCharacteristic;

        public string id
        {
            get
            {
                return mCharacteristic.Uuid.ToString();
            }
        }
        public string description
        {
            get
            {
                return mCharacteristic.UserDescription;
            }
        }

        //Event that is called when the value of the characteristic is changed
        private void CharacteristicEvent(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var buffer = args.CharacteristicValue;
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            _ValueChanged?.Invoke(sender, new CharacteristicEvent(data));
        }

        public CharacteristicBLE(GattCharacteristic pInput)
        {
            _ValueChanged = null;
            mCharacteristic = pInput;
            mCharacteristic.ValueChanged += CharacteristicEvent;
        }
        event ChangeEvent _ValueChanged;
        public event ChangeEvent ValueChanged
        {
            add
            {
                _ValueChanged += value;
            }
            remove
            {
                _ValueChanged -= value;
            }
        }
    }
    public class ClientBLE : IClientBLE
    {
        private List<IDeviceBLE> mVisibleDevices;
        private DeviceWatcher mDeviceWatcher;

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            await Task.Run(() =>
            {
                if (sender != mDeviceWatcher)
                    return;
                if (args.Name == string.Empty)
                    return;
                mVisibleDevices.Add(new UnPairedDeviceBLE() { id = args.Id, name = args.Name, paired = args.Pairing.IsPaired, CanPair = args.Pairing.CanPair, Information = args });
            });
        }
        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await Task.Run(() =>
            {
                if (sender != mDeviceWatcher)
                    return;

                var removed_id = args.Id;
                for (int i = 0; i < mVisibleDevices.Count; i++)
                {
                    var item = mVisibleDevices[i];
                    if (item.id == removed_id)
                    {
                        mVisibleDevices.Remove(item);
                        return;
                    }
                }
            });
        }
        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await Task.Run(() =>
            {
                if (sender != mDeviceWatcher)
                    return;

                var removed_id = args.Id;
                for (int i = 0; i < mVisibleDevices.Count; i++)
                {
                    var item = mVisibleDevices[i];

                }
            });
        }
        private async void DeviceWatcher_EnumComplete(DeviceWatcher sender, Object args)
        {
            await Task.Run(() =>
            {
                if (sender != mDeviceWatcher)
                    return;
            });
        }
        private async void DeviceWatcher_Stopped(DeviceWatcher sender, Object args)
        {
            await Task.Run(() =>
            {
                if (sender != mDeviceWatcher)
                    return;
            });
        }

        private string _Name;
        public string Name
        {
            private set
            {
                _Name = value;
            }
            get
            {
                return _Name;
            }
        }

        public List<IDeviceBLE> ListDevices()
        {
            return mVisibleDevices;
        }

        public async Task<IDeviceBLE> Connect(IDeviceBLE pInput)
        {
            if (pInput.GetType() == typeof(UnPairedDeviceBLE))
            {
                //Pair with the defice if needed.
                var input = pInput as UnPairedDeviceBLE;
                if (input.CanPair)
                {
                    var id = input.id;
                    var status = (await input.Information.Pairing.PairAsync()).Status;

                    if (!(status == DevicePairingResultStatus.AlreadyPaired || status == DevicePairingResultStatus.Paired))
                        return null;
                }

                //Get the bluetooth device from the UI thread
                var mDeviceBLE = (await BluetoothLEDevice.FromIdAsync(input.Information.Id));

                //Setup the services for the bluetooth device
                if (mDeviceBLE == null)
                    return null;

                return new PairedDeviceBLE(mDeviceBLE);
            }
            return null;
        }
        public bool Initialise()
        {
            throw new NotImplementedException();
        }

        public ClientBLE()
        {
            mVisibleDevices = new List<IDeviceBLE>();

            //Get all devices paired and not.
            string query1 = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState(true) + ")";
            string query2 = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState(false) + ")";
            var query = query1 + " OR " + query2;

            //Create device watcher
            mDeviceWatcher = DeviceInformation.CreateWatcher(query, new string[]{ "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" }, DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            mDeviceWatcher.Added += DeviceWatcher_Added;
            mDeviceWatcher.Updated += DeviceWatcher_Updated;
            mDeviceWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            mDeviceWatcher.EnumerationCompleted += DeviceWatcher_EnumComplete;
            mDeviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            mDeviceWatcher.Start();
        }
        ~ClientBLE()
        {
            if (mDeviceWatcher != null)
            {
                // Unregister the event handlers.
                mDeviceWatcher.Added -= DeviceWatcher_Added;
                mDeviceWatcher.Updated -= DeviceWatcher_Updated;
                mDeviceWatcher.Removed -= DeviceWatcher_Removed;
                mDeviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumComplete;
                mDeviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                mDeviceWatcher.Stop();
                mDeviceWatcher = null;
            }
        }
    }
}