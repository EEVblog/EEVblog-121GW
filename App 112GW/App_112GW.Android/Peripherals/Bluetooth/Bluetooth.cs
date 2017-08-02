using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using Android.Bluetooth;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;

namespace rMultiplatform.BLE
{
    public class ClientBLE : AClientBLE, IClientBLE
    {
        volatile private IBluetoothLE mDevice;
        volatile private IAdapter mAdapter;

        private static int index = 0;
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
            }, (indexer.ToString() + " Adding"));

            //
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
            }, "Reset");
        }

        IDeviceBLE ConnectingDevice = null;
        PairedDeviceBLE Device = null;
        private void ConnectionComplete(Task obj)
        {
            Debug.WriteLine("Scanning stopped, building services and charateristics.");
            Device = new PairedDeviceBLE((ConnectingDevice as UnPairedDeviceBLE).mDevice, () => { TriggerDeviceConnected(Device); });
        }
        private void StopScanning(Task obj)
        {
            Debug.WriteLine("Device connected, stopping scanning.");
            //ConnectionComplete(obj);
            mAdapter.StopScanningForDevicesAsync().ContinueWith(ConnectionComplete);
        }

        public void Connect(IDeviceBLE pInput)
        {
            if (pInput == null)
                return;

            var inputType = pInput.GetType();
            var searchType = typeof(UnPairedDeviceBLE);

            Device = null;
            if (inputType == searchType)
            {
                //Pair if the device is able to pair
                AcceptRescan = false;
                ConnectingDevice = pInput;

                Debug.WriteLine("Connecting to new device.");
                mAdapter.ConnectToDeviceAsync((ConnectingDevice as UnPairedDeviceBLE).mDevice).ContinueWith(StopScanning);
            }
        }

        public ClientBLE()
        {
            mVisibleDevices = new System.Collections.ObjectModel.ObservableCollection<IDeviceBLE>();

            //Setup bluetoth basic adapter
            mDevice     = CrossBluetoothLE.Current;
            mAdapter    = CrossBluetoothLE.Current.Adapter;
            mAdapter.ScanTimeout = 20000;
            mAdapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
            mAdapter.ScanTimeoutElapsed += MAdapter_ScanTimeoutElapsed;

            //Add debug state change indications
            mDevice.StateChanged += (s, e) => { Debug.WriteLine($"The bluetooth state changed to {e.NewState}"); };
            if (mDevice.IsOn && mDevice.IsAvailable)
                mAdapter.DeviceDiscovered += DeviceWatcher_Added;

            //Start the scan
            mAdapter.StartScanningForDevicesAsync();
        }

        private void MAdapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            mAdapter.ConnectToDeviceAsync(e.Device).Wait();
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