using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.BluetoothLE;
using System.Threading.Tasks;

namespace rMultiplatform.BLE
{
    public class UnPairedDeviceBLE : IDeviceBLE
    {
        public IDevice mDevice;
        public event SetupComplete Ready;
        public event ChangeEvent Change;
        
        public string Id
        {
            get
            {
                return mDevice.Uuid.ToString();
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
                return mDevice.PairingStatus == PairingStatus.Paired;
            }
        }
        public bool CanPair
        {
            get
            {
                return mDevice.IsPairingAvailable();
            }
        }

        public UnPairedDeviceBLE(IDevice pDevice)
        {
            mDevice = pDevice;
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
    public class PairedDeviceBLE : IDeviceBLE
    {
        private IDisposable wsd;
        private IDevice mDevice;
        private List<IServiceBLE> mServices;
        
        public string Id
        {
            get
            {
                return mDevice.Uuid.ToString();
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
                return mDevice.PairingStatus == PairingStatus.Paired;
            }
        }
        public bool CanPair
        {
            get
            {
                return mDevice.IsPairingAvailable();
            }
        }
        
        public event SetupComplete Ready;
        void Build()
        {
            wsd = mDevice.WhenServiceDiscovered().Subscribe(
            obj =>
            {
                mServices.Add(new ServiceBLE(obj, ()=> { }, InvokeChange));
            });
            Ready?.Invoke();
        }

        public event ChangeEvent Change;
        private void InvokeChange(object o, CharacteristicEvent v)
        {
            Change?.Invoke(o,v);
        }

        public PairedDeviceBLE(IDevice pDevice, SetupComplete ready)
        {
            Ready += ready;
            mServices = new List<IServiceBLE>();
            mDevice = pDevice;
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