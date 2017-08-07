using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using Android.Bluetooth;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;

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
                {
                    Debug.WriteLine(item.Name + " " + item.Id);
                    AddUniqueItem(new UnPairedDeviceBLE(item));
                }
            }, (indexer.ToString() + " Adding"));
        }

        private bool AcceptRescan = false;
        public void Start()
        {
            AcceptRescan = true;
            mAdapter.StartScanningForDevicesAsync();
        }
        public void Stop()
        {
            AcceptRescan = false;
            mAdapter.StopScanningForDevicesAsync().Wait();
        }
        public void Rescan()
        {
            if (AcceptRescan)
                mAdapter.StartScanningForDevicesAsync();
        }
        public void Reset()
        {
            Stop();
            Start();
        }

        IDeviceBLE ConnectingDevice = null;
        PairedDeviceBLE Device = null;
        private void ConnectionComplete(Task obj)
        {
            Debug.WriteLine("Connection Complete.");
            Device = new PairedDeviceBLE((ConnectingDevice as UnPairedDeviceBLE).mDevice, 
            (dev) => {
                TriggerDeviceConnected(dev);
            });
        }
        private void StopScanning(Task obj)
        {
            Debug.WriteLine("Stopping scanning.");
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
            //Setup bluetoth basic adapter
            mDevice = CrossBluetoothLE.Current;
            mAdapter = CrossBluetoothLE.Current.Adapter;
            mAdapter.ScanTimeoutElapsed += MAdapter_ScanTimeoutElapsed;

            //Add debug state change indications
            mDevice.StateChanged += (s, e) =>
            {
                Debug.WriteLine($"The bluetooth state changed to " + e.NewState.ToString());
                if (e.NewState == BluetoothState.TurningOn || e.NewState == BluetoothState.On)
                    Reset();
            };

            //
            if (mDevice.IsOn && mDevice.IsAvailable)
                mAdapter.DeviceDiscovered += DeviceWatcher_Added;
            mAdapter.DeviceConnectionLost += DeviceConnection_Lost;

            //Start the scan
            Start();
        }

        private void MAdapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("private void MAdapter_ScanTimeoutElapsed(object sender, EventArgs e).");
            Rescan();
        }

        private void DeviceConnection_Lost (object sender, DeviceErrorEventArgs e)
        {
            string disconnect_Id = e.Device.Id.ToString();
            Debug.WriteLine( "DeviceConnection_Lost." );
            Debug.WriteLine( disconnect_Id );
            foreach (var item in mConnectedDevices)
                if (item.Id == disconnect_Id)
                {
                    Debug.WriteLine(item.Id);
                    mAdapter.DisconnectDeviceAsync(e.Device).ContinueWith((temp) =>
                    {
                        mAdapter.ConnectToDeviceAsync(e.Device).ContinueWith((obj) => { item.Remake(e.Device); });
                    });
                }
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