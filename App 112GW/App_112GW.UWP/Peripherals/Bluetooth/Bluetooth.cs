using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        volatile public DeviceInformation Information;
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
        volatile private BluetoothLEDevice mDevice;
        private bool mSuccess;
        private List<IServiceBLE> mServices;

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

        private bool Build()
        {
            var servs = mDevice.GetGattServicesAsync().AsTask().Result.Services;

            mServices = new List<IServiceBLE>();
            mServices.Clear();
            foreach (var service in servs)
                mServices.Add(new ServiceBLE(service));

            return true;
        }
        public PairedDeviceBLE(BluetoothLEDevice pInput)
        {
            mDevice = pInput;
            mSuccess = Build();
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
        volatile private GattDeviceService mService;
        private List<ICharacteristicBLE> mCharacteristics;
        public List<ICharacteristicBLE> Characteristics
        {
            get
            {
                return mCharacteristics;
            }
        }
        private bool mSuccess;

        public string Id
        {
            get
            {
                return mService.Uuid.ToString();
            }
        }
        public override string ToString()
        {
            return Id;
        }
        private bool Build()
        {
            var items = mService.GetCharacteristicsAsync().AsTask().Result.Characteristics;

            mCharacteristics = new List<ICharacteristicBLE>();
            mCharacteristics.Clear();
            foreach (var item in items)
                mCharacteristics.Add(new CharacteristicBLE(item));

            return true;
        }
        public ServiceBLE(GattDeviceService pInput)
        {
            mService = pInput;
            mSuccess = Build();
        }
    }
    public class CharacteristicBLE : ICharacteristicBLE
    {
        volatile private GattCharacteristic  mCharacteristic;
        public string Id
        {
            get
            {
                return mCharacteristic.Uuid.ToString();
            }
        }
        public string Description
        {
            get
            {
                return mCharacteristic.UserDescription;
            }
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

        //Event that is called when the value of the characteristic is changed
        private void CharacteristicEvent_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Debug.WriteLine("CharateristicEvent : " + args.ToString());

            var buffer = args.CharacteristicValue;
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);

            var charEvent = new CharacteristicEvent(data);
            _ValueChanged?.Invoke(sender, charEvent);
        }
        public CharacteristicBLE(GattCharacteristic pInput)
        {
            _ValueChanged = null;
            mCharacteristic = pInput;
            mCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate).AsTask();
            mCharacteristic.ValueChanged += CharacteristicEvent_ValueChanged;
        }
    }

    public class ClientBLE : IClientBLE
    {
        volatile private List<IDeviceBLE> mVisibleDevices;
        volatile private DeviceWatcher mDeviceWatcher;

        private static int index = 0;
        private static Mutex mut = new Mutex();
        private void DeviceWatcher_Added        (DeviceWatcher sender, DeviceInformation args)
        {
            if (sender != mDeviceWatcher)
                return;
            if (args.Name == string.Empty)
                return;
            if (mVisibleDevices == null)
                return;

            var temp = new UnPairedDeviceBLE(args);

            int indexer = index++;
            string tag = indexer.ToString() + " Adding";

            Debug.WriteLine(tag + " : Waiting");
            mut.WaitOne();
            Debug.WriteLine(tag + " : Started");

            mVisibleDevices.Add(temp);

            Debug.WriteLine(tag + " : Done");
            mut.ReleaseMutex();
            Debug.WriteLine(tag + " : Released");
        }
        private void DeviceWatcher_Removed      (DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (sender != mDeviceWatcher)
                return;
            if (mVisibleDevices == null)
                return;

            int indexer = index++;
            string tag = indexer.ToString() + " Removed";

            Debug.WriteLine(tag + " : Waiting");
            mut.WaitOne();
            Debug.WriteLine(tag + " : Started");

            var removed_id = args.Id;
            for (int i = 0; i < mVisibleDevices.Count; i++)
            {
                var item = mVisibleDevices[i];
                if (item.Id == removed_id)
                    mVisibleDevices.RemoveAt(i);
            }

            Debug.WriteLine(tag + " : Done");
            mut.ReleaseMutex();
            Debug.WriteLine(tag + " : Released");
        }
        private void DeviceWatcher_Updated      (DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (sender != mDeviceWatcher)
                return;
        }

        private IDeviceBLE LastDevice = null;
        public List<IDeviceBLE> ListDevices()
        {
            return mVisibleDevices;
        }

        public IDeviceBLE Connect(IDeviceBLE pInput)
        {
            var inputType = pInput.GetType();
            var searchType = typeof(UnPairedDeviceBLE);
            if (inputType == searchType)
            {
                //Pair with the defice if needed.
                var input = pInput as UnPairedDeviceBLE;

                //Pair if the device is able to pair
                var     id = input.Id;
                var     result = input.Information.Pairing.PairAsync().AsTask().Result;
                var     status = result.Status;
                bool    paired = (
                    (status == DevicePairingResultStatus.AlreadyPaired) || 
                    (status == DevicePairingResultStatus.Paired));

                //Only create device if it is paired
                if (paired)
                {
                    //Get the bluetooth device from the UI thread
                    var mDeviceBLE = BluetoothLEDevice.FromIdAsync(input.Information.Id).AsTask().Result;
                    if (mDeviceBLE == null)
                        return null;

                    mVisibleDevices?.Clear();
                    mVisibleDevices = null;
                    return (LastDevice = new PairedDeviceBLE(mDeviceBLE));
                }
            }
            return null;
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
            mDeviceWatcher.Added                += DeviceWatcher_Added;
            mDeviceWatcher.Updated              += DeviceWatcher_Updated;
            mDeviceWatcher.Removed              += DeviceWatcher_Removed;

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

                // Stop the watcher.
                mDeviceWatcher.Stop();
                mDeviceWatcher = null;
            }
        }
    }
}