using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Windows.Security.Cryptography;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        volatile public DeviceInformation Information;
        public event SetupComplete Ready;
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
        private List<IServiceBLE> mServices;

        public event SetupComplete Ready;
        public event ChangeEvent Change;

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

        private void Build()
        {
            var servs = mDevice.GetGattServicesAsync().AsTask().Result.Services;

            mServices = new List<IServiceBLE>();
            mServices.Clear();
            foreach (var service in servs)
                mServices.Add(new ServiceBLE(service));

            Ready?.Invoke();
        }
        public PairedDeviceBLE(BluetoothLEDevice pInput, SetupComplete ready)
        {
            Ready = ready;
            mDevice = pInput;
            Build();
        }

        public override string ToString()
        {
            return Name + "\n" + Id;
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
        public event SetupComplete Ready;


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
        public event SetupComplete Ready;

        volatile private GattCharacteristic mCharacteristic;
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

        public bool Send(string pInput)
        {
            var temp = mCharacteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary(pInput, BinaryStringEncoding.Utf8)).AsTask().Result;
            return true;
        }
        public bool Send(byte[] pInput)
        {
            var temp = mCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(pInput)).AsTask().Result;
            return true;
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

    public class ClientBLE : AClientBLE, IClientBLE
    {
        volatile private DeviceWatcher mDeviceWatcher;
        private static int index = 0;
        private void DeviceWatcher_Added    (DeviceWatcher sender, DeviceInformation        args)
        {
            if (sender != mDeviceWatcher)
                return;
            if (args.Name == string.Empty)
                return;
            if (mVisibleDevices == null)
                return;

            var temp = new UnPairedDeviceBLE(args);
            MutexBlock(() =>
            {
                AddUniqueItem(temp);
            }, ((index++).ToString() + " Adding"));
        }
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (sender != mDeviceWatcher)
                return;
            if (mVisibleDevices == null)
                return;

            MutexBlock(() =>
            {
                var removed_id = args.Id;
                for (int i = 0; i < mVisibleDevices.Count; i++)
                {
                    var item = mVisibleDevices[i];
                    if (item.Id == removed_id)
                        (mVisibleDevices[i] as UnPairedDeviceBLE).Information.Update(args);
                }
            }, ((index++).ToString() + " Removed"));
            TriggerListUpdate();
        }

        private void DeviceWatcher_Removed  (DeviceWatcher sender, DeviceInformationUpdate  args)
        {
            if (sender != mDeviceWatcher)
                return;
            if (mVisibleDevices == null)
                return;

            MutexBlock(() =>
            {
                var removed_id = args.Id;
                for (int i = 0; i < mVisibleDevices.Count; i++)
                {
                    var item = mVisibleDevices[i];
                    if (item.Id == removed_id)
                        mVisibleDevices.RemoveAt(i);
                }
            }, ((index++).ToString() + " Removed"));
            TriggerListUpdate();
        }

        private void PairAsync ( UnPairedDeviceBLE pInput )
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                pInput.Information.Pairing.PairAsync().AsTask().ContinueWith((arg) => { GetDevice(pInput); });
            });
        }
        
        private void GetDevice( UnPairedDeviceBLE input )
        {
            var mDeviceBLE = BluetoothLEDevice.FromIdAsync(input.Information.Id).AsTask().Result;
            if (mDeviceBLE == null)
                return;

            bool setup = false;
            var NewPairedDevice = new PairedDeviceBLE(mDeviceBLE, () => { setup = true; });
            if (setup)
                TriggerDeviceConnected(NewPairedDevice);
        }

        public void Connect(IDeviceBLE pInput)
        {
            var inputType = pInput.GetType();
            var searchType = typeof(UnPairedDeviceBLE);
            if ( inputType == searchType )
            {
                //Pair with the defice if needed.
                var input = pInput as UnPairedDeviceBLE;

                //Pair if the device is able to pair
                var id = input.Id;

                //Only create device if it is paired
                if (input.Paired)
                    GetDevice(input);
                else
                    PairAsync(input);
            }
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
            mDeviceWatcher.Stopped += MDeviceWatcher_Stopped;

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
                mDeviceWatcher.Stopped -= MDeviceWatcher_Stopped;

                // Stop the watcher.
                mDeviceWatcher.Stop();
            }
        }

        public void Start()
        {
            try
            {
                mDeviceWatcher.Start();
            }
            catch { }
        }
        public void Stop()
        {
            mDeviceWatcher.Stop();
        }
        public void Rescan()
        {
            Reset();
        }
        public void Reset()
        {
            if (mDeviceWatcher.Status == DeviceWatcherStatus.Stopped)
                mDeviceWatcher.Start();
        }
        private void MDeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            Debug.WriteLine("Restarting Watcher!");
            mDeviceWatcher.Start();
        }
    }
}