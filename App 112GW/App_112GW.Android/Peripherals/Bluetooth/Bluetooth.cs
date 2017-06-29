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

        public event SetupComplete Ready;

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
        private List<IServiceBLE> mServices;

        public event SetupComplete Ready;

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

        void Build()
        {
            var servs = mDevice.GetServicesAsync().ContinueWith(AddServices);
        }

        private int UninitialisedServices = 0;
        


        void AddServices(Task<IList<IService>> obj)
        {
            UninitialisedServices = obj.Result.Count;
            foreach (var item in obj.Result)
            {
                Debug.WriteLine("Service adding : " + item.Name);
                var temp = new ServiceBLE(item, ServiceReady);
                mServices.Add(temp);
            }
        }

        private void ServiceReady()
        {
            --UninitialisedServices;
            if (UninitialisedServices == 0)
            {
                Debug.WriteLine("Services finished setting up : " + Id);
                Ready?.Invoke();
            }
        }

        public PairedDeviceBLE(IDevice pDevice, SetupComplete ready)
        {
            mServices = new List<IServiceBLE>();
            mDevice = pDevice;
            Ready += ready;
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

        volatile private IService           mService;
        private List<ICharacteristicBLE>    mCharacteristics;
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

        private void Build()
        {
            mService.GetCharacteristicsAsync().ContinueWith(AddCharacteristics);
        }
        private void AddCharacteristics(Task<IList<ICharacteristic>> obj)
        {
            try
            {
                UninitialisedServices = obj.Result.Count;
                foreach (var item in obj.Result)
                {
                    Debug.WriteLine("Characteristic adding : " + item.Name);
                    var temp = new CharacteristicBLE(item, CharateristicReady);
                    mCharacteristics.Add(temp);
                }
            }
            catch ( Exception e )
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
        }

        private int UninitialisedServices = 0;
        private void CharateristicReady()
        {
            --UninitialisedServices;
            if (UninitialisedServices == 0)
            {
                Debug.WriteLine("Characteristics finished setting up : " + Id);
                Ready?.Invoke();
            }
        }

        public ServiceBLE(IService pInput, SetupComplete ready)
        {
            mCharacteristics = new List<ICharacteristicBLE>();
            if (ready != null)
                Ready += ready;

            mService = pInput;
            Build();
        }
    }

    public class CharacteristicBLE : ICharacteristicBLE
    {
        volatile public ICharacteristic mCharacteristic;

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
        public event SetupComplete Ready;

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
        private void CharacteristicEvent_ValueChanged (object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
        {
            Debug.WriteLine("CharateristicEvent : " + args.ToString());
            
            if (args == null)
                Debug.WriteLine("Args is null.");
            if (args.Characteristic == null)
                Debug.WriteLine("Args.Characteristic is null.");
            if (args.Characteristic.Value == null)
                Debug.WriteLine("Args.Characteristic.Value is null.");
            if (_ValueChanged == null)
                Debug.WriteLine("ValueChanged is null.");

            var buffer = args.Characteristic.Value;
            var charEvent = new CharacteristicEvent(buffer);
            _ValueChanged?.Invoke(sender, charEvent);
        }
        public CharacteristicBLE(ICharacteristic pInput, SetupComplete ready)
        {
            Ready += ready;

            Debug.WriteLine("Setting up characteristic");

            mCharacteristic = pInput;
            mCharacteristic.ValueUpdated += CharacteristicEvent_ValueChanged;

            if (mCharacteristic.CanUpdate)
                mCharacteristic.StartUpdatesAsync().ContinueWith(UpdateStarted);
            else
                Ready?.Invoke();

            _ValueChanged = null;
        }
        private void UpdateStarted(Task obj)
        {
            Debug.WriteLine("Characteristic updates started.");
            Ready?.Invoke();
        }
    }

    public class ClientBLE : AClientBLE, IClientBLE
    {
        volatile private IBluetoothLE mDevice;
        volatile private IAdapter mAdapter;

        private static int index = 0;
        private void DeviceWatcher_Added(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs args)
        {
            Debug.WriteLine("Device watcher detected : " + args.Device.Name);

            int indexer = index++;
            if (args.Device.Name == string.Empty || mVisibleDevices == null)
                return;

            MutexBlock(() => 
            {
                mVisibleDevices.Clear();
                foreach (var item in mAdapter.DiscoveredDevices)
                    AddUniqueItem(new UnPairedDeviceBLE(item));
            }, (indexer.ToString() + " Adding"));

            TriggerListUpdate();
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
                AcceptRescan = true;
                if (mAdapter.IsScanning == false)
                    mAdapter.StartScanningForDevicesAsync();
            }, "Reset" );
        }

        IDeviceBLE      ConnectingDevice = null;
        PairedDeviceBLE Device = null;
        private void ConnectionComplete ( Task obj )
        {
            Debug.WriteLine("Scanning stopped, building services and charateristics.");
            Device = new PairedDeviceBLE((ConnectingDevice as UnPairedDeviceBLE).mDevice, () => { TriggerDeviceConnected(Device); });
        }
        private void StopScanning ( Task obj )
        {
            Debug.WriteLine("Device connected, stopping scanning.");
            mAdapter.StopScanningForDevicesAsync().ContinueWith(ConnectionComplete);
        }

        public void Connect ( IDeviceBLE pInput )
        {
            if (pInput == null)
                return;

            var inputType = pInput.GetType();
            var searchType = typeof( UnPairedDeviceBLE );

            Device = null;
            if ( inputType == searchType )
            {
                //Pair if the device is able to pair
                AcceptRescan = false;
                ConnectingDevice = pInput;

                Debug.WriteLine( "Connecting to new device." );
                mAdapter.ConnectToDeviceAsync( (ConnectingDevice as UnPairedDeviceBLE).mDevice ).ContinueWith( StopScanning );
            }
        }

        public ClientBLE()
        {
            mVisibleDevices = new List<IDeviceBLE>();

            //Setup bluetoth basic adapter
            mDevice                     =   CrossBluetoothLE.Current;
            mAdapter                    =   CrossBluetoothLE.Current.Adapter;
            mAdapter.ScanTimeout        =   20000;
            mAdapter.ScanMode           =   Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
            mAdapter.ScanTimeoutElapsed +=  MAdapter_ScanTimeoutElapsed;

            //Add debug state change indications
            mDevice.StateChanged += (s, e) => {Debug.WriteLine($"The bluetooth state changed to {e.NewState}");};
            if (mDevice.IsOn && mDevice.IsAvailable)
                mAdapter.DeviceDiscovered += DeviceWatcher_Added;

            //Start the scan
            mAdapter.StartScanningForDevicesAsync();
        }

        private void MAdapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            Debug.WriteLine("Connection to device lost.");
            mAdapter.ConnectToDeviceAsync(e.Device).Wait();
        }
        private void MAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("Scan time elapsed.");
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