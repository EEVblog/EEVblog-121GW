using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
    public interface        IDeviceBLE
    {
        string  Id { get; }
        string  Name { get; }
        bool    Paired { get; }
        bool    CanPair { get; }

        string ToString();
        List<IServiceBLE> Services { get; }
    }
    public interface        IServiceBLE
    {
        string Id { get; }
        string ToString();

        List<ICharacteristicBLE> Characteristics { get;}
    }
    public interface        ICharacteristicBLE
    {
        string Id { get; }
        string Description { get; }

        bool Send( string pInput );
        bool Send( byte[] pInput );

        event ChangeEvent ValueChanged;
    }
    public interface        IClientBLE
    {
        //Does not return a usable device, it must be paired first
        List<IDeviceBLE> ListDevices();
        IDeviceBLE Connect(IDeviceBLE pInput);
    }
}
