using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace rMultiplatform.BLE
{
    public delegate void ChangeEvent(Object o, CharacteristicEvent v);
    public class CharacteristicEvent : EventArgs
    {
        public string NewValue;
        public byte[] Bytes;
        public CharacteristicEvent(byte[] pNewValue)
        {
            Bytes = pNewValue;
            NewValue = Encoding.UTF8.GetString(Bytes);
        }
    }

    public interface IDeviceBLE
    {
        string id { get; }
        string name { get; }
        bool paired { get; }
        bool CanPair { get; }
        string ToString();

        List<IServiceBLE> Services { get; }
    }
    public interface IServiceBLE
    {
        string id { get; }
        string ToString();

        List<ICharacteristicBLE> Characteristics { get;}
    }
    public interface ICharacteristicBLE
    {
        string id { get; }
        string description { get; }

        event ChangeEvent ValueChanged;
    }
    public interface IClientBLE
    {
        string Name{ get;}

        //Does not return a usable device, it must be paired first
        List<IDeviceBLE> ListDevices();
        Task<IDeviceBLE> Connect(IDeviceBLE pInput);

        bool Initialise();
    }
}
