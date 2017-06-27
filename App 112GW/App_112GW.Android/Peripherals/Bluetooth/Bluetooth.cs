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

        private bool Build()
        {
            var servs = mDevice.GetServicesAsync().Result;
            mServices = new List<IServiceBLE>();
            mServices.Clear();
            foreach (var service in servs)
                mServices.Add(new ServiceBLE(service));
            return true;
        }
        public PairedDeviceBLE(IDevice pDevice)
        {
            mDevice = pDevice;
            mSuccess = Build();
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
        private bool Build()
        {
            var items = mService.GetCharacteristicsAsync().Result;

            mCharacteristics = new List<ICharacteristicBLE>();
            mCharacteristics.Clear();
            foreach (var item in items)
                mCharacteristics.Add(new CharacteristicBLE(item));

            return true;
        }
        public ServiceBLE(IService pInput)
        {
            mService = pInput;
            mSuccess = Build();
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

            try
            {
                mCharacteristic.StartUpdatesAsync().Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }














    public class ClientBLE : IClientBLE
    {
        public event VoidEvent DeviceListUpdated;

        volatile public List<IDeviceBLE> mVisibleDevices;
        volatile private IBluetoothLE mDevice;
        volatile private IAdapter mAdapter;

        private static int index = 0;
        private static Mutex mut = new Mutex();
        
        void MutexBlock(Action Function, string tag = "")
        {
            void GetMutex(string ltag = "")
            {
                Debug.WriteLine(ltag + " : Waiting");
                mut.WaitOne();
                Debug.WriteLine(ltag + " : Started");
            }
            void ReleaseMutex(string ltag = "")
            {
                Debug.WriteLine(ltag + " : Done");
                mut.ReleaseMutex();
                Debug.WriteLine(ltag + " : Released");
            }

            try
            {
                GetMutex(tag);
                Task.Run(Function).Wait();
                ReleaseMutex(tag);
            }
            catch (Exception e)
            {
                ReleaseMutex(tag);
            }
        }
        void AddUniqueItem(IDeviceBLE pInput)
        {
            if (pInput.Name != null)
                if (pInput.Name.Length > 0)
                {
                    bool add = true;
                    foreach (var device in mVisibleDevices)
                        if (device.Id == pInput.Id)
                            add = false;
                    if (add)
                        mVisibleDevices.Add(pInput);
                }
        }

        private void DeviceWatcher_Added(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs args)
        {
            int indexer = index++;
            if (args.Device.Name == string.Empty || mVisibleDevices == null)
                return;

            MutexBlock(() => 
            {
                mVisibleDevices.Clear();
                foreach (var item in mAdapter.DiscoveredDevices)
                    AddUniqueItem(new UnPairedDeviceBLE(item));
            } , (indexer.ToString() + " Adding"));

            DeviceListUpdated.Invoke();
        }
        
        public List<IDeviceBLE> ListDevices()
        {
            if (mVisibleDevices == null)
                return new List<IDeviceBLE>();
            return mVisibleDevices;
        }

        private bool AcceptRescan = false;
        public void Start()
        {
            MutexBlock(() =>
            {
                AcceptRescan = true;
                mAdapter.StartScanningForDevicesAsync();
            }, "Start");
        }
        public void Stop()
        {
            MutexBlock(() =>
            {
                AcceptRescan = false;
                mAdapter.StopScanningForDevicesAsync().Wait();
            }, "Stop");
        }


        public void Rescan()
        {
            if (AcceptRescan)
            {
                MutexBlock(() =>
                {
                    mAdapter.StopScanningForDevicesAsync().Wait();
                    mAdapter.StartScanningForDevicesAsync();
                }, "Rescan");
            }
        }
        public void Reset()
        {
            MutexBlock(() =>
            {
                DeviceListUpdated?.Invoke();
                AcceptRescan = true;
                if (mAdapter.IsScanning == false)
                    mAdapter.StartScanningForDevicesAsync();
            }, "Reset");
        }

        public IDeviceBLE Connect(IDeviceBLE pInput)
        {
            PairedDeviceBLE Device = null;
            var inputType = pInput.GetType();
            var searchType = typeof(UnPairedDeviceBLE);

            if (inputType == searchType)
            {
                //Pair with the defice if needed.
                var input = pInput as UnPairedDeviceBLE;

                //Pair if the device is able to pair
                try
                {
                    MutexBlock(() =>
                    {
                        //Stop existing 
                        AcceptRescan = false;

                        mAdapter.StopScanningForDevicesAsync().Wait();

                        //var systemDevices = mAdapter.GetSystemConnectedOrPairedDevices();
                        //foreach (var device in systemDevices)
                        //    mAdapter.ConnectToDeviceAsync(device);

                        mAdapter.ConnectToDeviceAsync(input.mDevice).Wait();

                        //Add device
                        Device = new PairedDeviceBLE(input.mDevice);
                    }, "Connecting");
                }
                catch (DeviceConnectionException e)
                {
                    Debug.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return Device;
        }
        public ClientBLE()
        {
            mVisibleDevices = new List<IDeviceBLE>();

            //Setup bluetoth basic adapter
            mDevice = CrossBluetoothLE.Current;
            mAdapter = CrossBluetoothLE.Current.Adapter;
            mAdapter.ScanTimeout = 20000;
            mAdapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
            mAdapter.ScanTimeoutElapsed += MAdapter_ScanTimeoutElapsed;

            //Add debug state change indications
            mDevice.StateChanged += (s, e) => {Debug.WriteLine($"The bluetooth state changed to {e.NewState}");};

            if (mDevice.IsOn && mDevice.IsAvailable)
                mAdapter.DeviceDiscovered += DeviceWatcher_Added;

            //Start the scan
            mAdapter.StartScanningForDevicesAsync();
        }

        private void MAdapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            mAdapter.ConnectToDeviceAsync(e.Device);
        }

        private void MAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Rescan();
        }
        ~ClientBLE()
        {
            Debug.WriteLine("Deconstructing ClientBLE!");
            try
            {
                Stop();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}