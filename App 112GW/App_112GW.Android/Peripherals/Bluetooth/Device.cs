using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        public IDevice              mDevice;
        public event SetupComplete  Ready;
        public event ChangeEvent    Change;

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
                if (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected)
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
        private IDevice             mDevice;
        private List<IServiceBLE>   mServices;
        public event SetupComplete  Ready;

        public event ChangeEvent    Change;
        private void InvokeChange(object o, CharacteristicEvent v)
        {
            Change?.Invoke(o, v);
        }


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
                if (mDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected)
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
            var servs = mDevice.GetServicesAsync().ContinueWith( AddServices );
        }
        private int UninitialisedServices = 0;
        void AddServices(Task<IList<IService>> obj)
        {
            UninitialisedServices = obj.Result.Count;
            foreach (var item in obj.Result)
            {
                Debug.WriteLine("Service adding : " + item.Name);
                mServices.Add(new ServiceBLE(item, ServiceReady, InvokeChange));
            }
        }
        private void ServiceReady()
        {
            --UninitialisedServices;
            if (UninitialisedServices == 0)
                Ready?.Invoke();
        }
        public PairedDeviceBLE(IDevice pDevice, SetupComplete ready)
        {
            mServices   = new List<IServiceBLE>();
            mDevice     = pDevice;
            Ready       += ready;
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
}