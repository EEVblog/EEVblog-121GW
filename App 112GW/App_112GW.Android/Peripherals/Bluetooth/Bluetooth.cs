using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using Plugin.BLE;
using Plugin.BLE.Android;
using Plugin.BLE.Abstractions.Contracts;
using Android.Bluetooth;
using Plugin.BLE.Abstractions.Exceptions;
using System.Threading.Tasks;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        public IDevice mDevice;

        public string Id
        {
            get
            {
                return mDevice.Id.ToString();
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
                if (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected || mDevice.State == Plugin.BLE.Abstractions.DeviceState.Limited)
                    return true;
                return false;
            }
        }
        public bool CanPair
        {
            get
            {
                return true;
            }
        }
        public UnPairedDeviceBLE(IDevice pDevice) { mDevice = pDevice; }
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
        private IDevice mDevice;
        private bool mSuccess;
        private List<IServiceBLE> mServices;

        public string Id
        {
            get
            {
                return mDevice.Id.ToString();
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
                if (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected || mDevice.State == Plugin.BLE.Abstractions.DeviceState.Limited)
                    return true;
                return false;
            }
        }
        public bool CanPair
        {
            get
            {
                return true;
            }
        }

        private async Task<bool> Build()
        {
            var servs = await mDevice.GetServicesAsync();

            mServices = new List<IServiceBLE>();
            mServices.Clear();
            foreach (var service in servs)
                mServices.Add(new ServiceBLE(service));

            return true;
        }
        public PairedDeviceBLE(IDevice pDevice)
        {
            mDevice = pDevice;
            mSuccess = Build().Result;
        }

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











    public class ServiceBLE : IServiceBLE
    {
        volatile private IService mService;
        private bool mSuccess;
        private List<ICharacteristicBLE> mCharacteristics;
        public List<ICharacteristicBLE> Characteristics
        {
            get
            {
                return mCharacteristics;
            }
        }

        public string Id
        {
            get
            {
                return mService.Id.ToString();
            }
        }
        public override string ToString()
        {
            return Id;
        }
        private async Task<bool> Build()
        {
            var items = await mService.GetCharacteristicsAsync();

            mCharacteristics = new List<ICharacteristicBLE>();
            mCharacteristics.Clear();
            foreach (var item in items)
                mCharacteristics.Add(new CharacteristicBLE(item));

            return true;
        }
        public ServiceBLE(IService pInput)
        {
            mService = pInput;
            mSuccess = Build().Result;
        }
    }













    public class CharacteristicBLE : ICharacteristicBLE
    {
        volatile private ICharacteristic mCharacteristic;

        //volatile private GattCharacteristic mCharacteristic;
        public string Id
        {
            get
            {
                return mCharacteristic.Id.ToString();
            }
        }
        public string Description
        {
            get
            {
                return mCharacteristic.Name;
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
            byte[] bArray = Encoding.UTF8.GetBytes(pInput);
            return Send(bArray);
        }
        public bool Send(byte[] pInput)
        {
            var temp = mCharacteristic.WriteAsync(pInput).Result;
            return temp;
        }

        //Event that is called when the value of the characteristic is changed
        private void CharacteristicEvent_ValueChanged(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
        {
            Debug.WriteLine("CharateristicEvent : " + args.ToString());

            var buffer = args.Characteristic.Value;
            var charEvent = new CharacteristicEvent(buffer);
            _ValueChanged?.Invoke(sender, charEvent);
        }
        public CharacteristicBLE(ICharacteristic pInput)
        {
            _ValueChanged = null;
            mCharacteristic = pInput;
            mCharacteristic.ValueUpdated += CharacteristicEvent_ValueChanged;

            mCharacteristic.StartUpdatesAsync().Wait();
        }
    }














    public class ClientBLE : IClientBLE
    {
        volatile public List<IDeviceBLE> mVisibleDevices;
        volatile private IBluetoothLE mDevice;
        volatile private IAdapter mAdapter;

        private static int index = 0;
        private static Mutex mut = new Mutex();

        private void DeviceWatcher_Added(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs args)
        {
            if (args.Device.Name == string.Empty)
                return;
            if (mVisibleDevices == null)
                return;

            var temp = new UnPairedDeviceBLE (args.Device);

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

        private IDeviceBLE LastDevice = null;
        public List<IDeviceBLE> ListDevices()
        {
            if (mVisibleDevices == null)
                return new List<IDeviceBLE>();
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
                bool connected = false;
                try
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        mAdapter.ConnectToDeviceAsync(input.mDevice).Wait();
                    });
                    connected = true;
                }
                catch (DeviceConnectionException e)
                {
                    // ... could not connect to device
                    Debug.WriteLine(e.Message);
                }

                bool paired = connected;

                //Only create device if it is paired
                if (paired)
                {
                    mVisibleDevices?.Clear();
                    mVisibleDevices = null;
                    LastDevice = new PairedDeviceBLE(input.mDevice);
                    return (LastDevice);
                }
            }
            return null;
        }
        public ClientBLE()
        {
            mVisibleDevices = new List<IDeviceBLE>();

            //Setup bluetoth basic adapter
            mDevice = CrossBluetoothLE.Current;
            mAdapter = CrossBluetoothLE.Current.Adapter;

            //Add debug state change indications
            mDevice.StateChanged += (s, e) => {Debug.WriteLine($"The bluetooth state changed to {e.NewState}");};

            if (mDevice.IsOn && mDevice.IsAvailable)
                mAdapter.DeviceDiscovered += DeviceWatcher_Added;

            mDevice.Adapter.StartScanningForDevicesAsync();

            ////Get all devices paired and not.
            //string query1 = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState(true) + ")";
            //string query2 = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState(false) + ")";
            //var query = query1 + " OR " + query2;

            ////Create device watcher
            //mDeviceWatcher = DeviceInformation.CreateWatcher(query, new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" }, DeviceInformationKind.AssociationEndpoint);

            //// Register event handlers before starting the watcher.
            //// Added, Updated and Removed are required to get all nearby devices
            //mDeviceWatcher.Added += DeviceWatcher_Added;
            //mDeviceWatcher.Updated += DeviceWatcher_Updated;
            //mDeviceWatcher.Removed += DeviceWatcher_Removed;

            // Start the watcher.
            //mDeviceWatcher.Start();
        }

        ~ClientBLE()
        {
            if (mDevice != null)
            {
                // Unregister the event handlers.
                //mDeviceWatcher.Added -= DeviceWatcher_Added;
                //mDeviceWatcher.Updated -= DeviceWatcher_Updated;
                //mDeviceWatcher.Removed -= DeviceWatcher_Removed;

                // Stop the watcher.
                //mDeviceWatcher.Stop();
                //mDeviceWatcher = null;
            }
        }
    }
}