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
    public class ClientBLE : AClientBLE, IClientBLE
    {
        private static int index = 0;
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            try
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
                    Debug.WriteLine(args.Name);
                    AddUniqueItem(temp);
                }, ((index++).ToString() + " Adding"));
            }
            catch { Debug.WriteLine("Caught Error : private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)"); }
        }
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            try
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
                }, ((index++).ToString() + " Updated"));
            }
            catch
            {
                Debug.WriteLine("Caught Error : private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)");
            }
        }
        private void DeviceWatcher_Removed  (DeviceWatcher sender, DeviceInformationUpdate  args)
        {
            //This aborts device removal.
            return;


            try
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
                            RunMainThread(() => { mVisibleDevices.Remove(item); });
                    }
                }, ((index++).ToString() + " Removed"));
            }
            catch
            {
                Debug.WriteLine("Caught Error : private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate  args)");
            }
        }

        private void ConnectionComplete( UnPairedDeviceBLE input )
        {
            BluetoothLEDevice.FromIdAsync(input.Information.Id).AsTask().ContinueWith(
            (obj)=>
            {
                Debug.WriteLine("Connection Complete.");
                if (obj.Result == null)
                    return;

                var temp = new PairedDeviceBLE(obj.Result, (dev) =>
                {
                    Debug.WriteLine("Enumeration Complete.");
                    TriggerDeviceConnected(dev);
                });
            });
        }
        public void Connect(IDeviceBLE pInput)
        {
            Debug.WriteLine("Connecting to : " + pInput.Id);

            var inputType = pInput.GetType();
            var searchType = typeof(UnPairedDeviceBLE);

            if (inputType == searchType)
                ConnectionComplete(pInput as UnPairedDeviceBLE);
        }

        private DeviceWatcher mDeviceWatcher;
        public void Start()
        {
            mDeviceWatcher.Start();
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
        {}

        public ClientBLE()
        {
            //Get all devices paired and not.
            string query1 = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState(true) + ")";
            string query2 = "(" + BluetoothLEDevice.GetDeviceSelectorFromPairingState(false) + ")";
            var query = query1 + " OR " + query2;

            //Create device watcher
            mDeviceWatcher = DeviceInformation.CreateWatcher(query, new string[]{ "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" }, DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            mDeviceWatcher.Added    +=  DeviceWatcher_Added;
            mDeviceWatcher.Updated  +=  DeviceWatcher_Updated;
            mDeviceWatcher.Removed  +=  DeviceWatcher_Removed;

            // Start the watcher.
            Start();
        }
        ~ClientBLE()
        {
            if (mDeviceWatcher != null)
            {
                // Unregister the event handlers.
                mDeviceWatcher.Added    -=  DeviceWatcher_Added;
                mDeviceWatcher.Updated  -=  DeviceWatcher_Updated;
                mDeviceWatcher.Removed  -=  DeviceWatcher_Removed;

                // Stop the watcher.
                Stop();
            }
        }
    }
}