using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BluetoothLE;

namespace rMultiplatform.BLE
{
    public class ClientBLE : AClientBLE, IClientBLE
    {
        private bool AcceptRescan = false;
        public void Start()
        {
            MutexBlock(() =>
            {
                AcceptRescan = true;
            }, "Start");
        }
        public void Stop()
        {
            MutexBlock(() =>
            {

            }, "Stop");
        }
        public void Rescan()
        {
            if (AcceptRescan)
            {
                MutexBlock(() =>
                {

                }, "Rescan");
            }
        }
        public void Reset()
        {
            MutexBlock(() =>
            {
                AcceptRescan = true;
            }, "Reset" );
        }

        IDeviceBLE ConnectingDevice = null;
        PairedDeviceBLE Device = null;
        private IDisposable dcs;
        private IDisposable ccss;
        
        private void ConnectionComplete ( Object obj )
        {
            if (obj == null)
                return;

            Debug.WriteLine("Scanning stopped, building services and charateristics.");
            var devtemp = (ConnectingDevice as UnPairedDeviceBLE);
            
            Device = new PairedDeviceBLE(devtemp.mDevice, null);
            TriggerDeviceConnected(Device);
        }
        
        public void Connect(IDeviceBLE pInput)
        {
            Debug.WriteLine("public void Connect(IDeviceBLE pInput)");
            if (pInput == null)
                return;

            var unpaired = (pInput as UnPairedDeviceBLE);
            var device = unpaired.mDevice;

            ConnectingDevice = unpaired;
            dcs = device.Connect().Subscribe((obj) => { ConnectionComplete(obj); });
        }

        private static int index = 0;
        private void DeviceSubscriber_Added ( IScanResult scanResult )
        {
            Debug.WriteLine("private void DeviceSubscriber_Added ( IScanResult scanResult )");
            var device = scanResult.Device;

            int indexer = index++;
            if (device.Name == string.Empty || mVisibleDevices == null)
                return;

            if(AddUniqueItem(new UnPairedDeviceBLE(device)))
                TriggerListUpdate();
        }

        public ClientBLE()
        {
            mVisibleDevices = new System.Collections.ObjectModel.ObservableCollection<IDeviceBLE>();

            //Setup 
            try
            {
                CrossBleAdapter.Current.ScanInterval(new TimeSpan(0, 0, 10));
                ccss = CrossBleAdapter.Current.Scan().Subscribe(ScanResult => { DeviceSubscriber_Added(ScanResult); });
            }
            catch { }
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