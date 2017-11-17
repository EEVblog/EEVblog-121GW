using System;
using Android.Bluetooth;


namespace Plugin.BluetoothLE.Internals
{
    public class GattEventArgs : EventArgs
    {
        public GattEventArgs(BluetoothGatt gatt, GattStatus status)
        {
            this.Gatt = gatt;
            this.Status = status;
        }


        public BluetoothGatt Gatt { get;  }
        public GattStatus Status { get; }
        public bool IsSuccessful => this.Status == GattStatus.Success;
    }
}