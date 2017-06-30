using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace rMultiplatform.BLE
{
    public delegate void    ChangeEvent(Object o, CharacteristicEvent v);
    public delegate void    DeviceConnected(IDeviceBLE device);

    public class            CharacteristicEvent : EventArgs
    {
        public string NewValue;
        public byte[] Bytes;

        public CharacteristicEvent(byte[] pNewValue)
        {
            Bytes = pNewValue;
            NewValue = Encoding.UTF8.GetString(Bytes);
        }
    }


    public delegate void SetupComplete();
    public interface        IDeviceBLE
    {
        event SetupComplete Ready;
        event ChangeEvent Change;

        string  Id { get; }
        string  Name { get; }
        bool    Paired { get; }
        bool    CanPair { get; }

        string ToString();
        List<IServiceBLE> Services { get; }
    }
    public interface        IServiceBLE
    {
        event SetupComplete Ready;

        string Id { get; }
        string ToString();

        List<ICharacteristicBLE> Characteristics { get;}
    }
    public interface        ICharacteristicBLE
    {
        event SetupComplete Ready;

        string Id { get; }
        string Description { get; }

        bool Send( string pInput );
        bool Send( byte[] pInput );

        event ChangeEvent ValueChanged;
    }

    public delegate void VoidEvent();
    public delegate void ConnectedEvent(IDeviceBLE pDevice);
    public interface        IClientBLE
    {
        event VoidEvent DeviceListUpdated;
        event ConnectedEvent DeviceConnected;

        //Default functions
        void Start();
        void Stop();
        void Rescan();
        void Reset();

        //Does not return a usable device, it must be paired first
        List<IDeviceBLE> ListDevices();
        void Connect(IDeviceBLE pInput);
    }

    public abstract class AClientBLE
    {
        volatile public List<IDeviceBLE> mVisibleDevices;

        private Mutex mut = new Mutex();
        public event VoidEvent DeviceListUpdated;
        public event ConnectedEvent DeviceConnected;

        public void TriggerListUpdate()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DeviceListUpdated?.Invoke();
            });
        }
        public void TriggerDeviceConnected(IDeviceBLE pInput)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("Finished connecting to : " + pInput.Id);
                DeviceConnected?.Invoke(pInput);
            });
        }

        public void MutexBlock(Action Function, string tag = "")
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
        public bool AddUniqueItem(IDeviceBLE pInput)
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

                    if (add)
                        TriggerListUpdate();

                    return add;
                }
            return false;
        }
        public List<IDeviceBLE> ListDevices()
        {
            if (mVisibleDevices == null)
                return new List<IDeviceBLE>();
            return mVisibleDevices;
        }
        public void RunMainThread(Action input)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                input?.Invoke();
            });
        }
    }
}
