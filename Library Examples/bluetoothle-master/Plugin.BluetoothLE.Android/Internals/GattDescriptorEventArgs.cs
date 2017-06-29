using System;
using Android.Bluetooth;


namespace Plugin.BluetoothLE.Internals
{
    public class GattDescriptorEventArgs : GattEventArgs
    {
        public BluetoothGattDescriptor Descriptor { get; }


        public GattDescriptorEventArgs(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status) : base(gatt, status)
        {
            this.Descriptor = descriptor;
        }
    }
}