using System;
using Android.Bluetooth;


namespace Plugin.BluetoothLE.Server.Internals
{
    public class GattRequestEventArgs : GattEventArgs
    {
        public GattRequestEventArgs(BluetoothDevice device, int requestId, int offset) : base(device)
        {
            this.RequestId = requestId;
            this.Offset = offset;
        }


        public int RequestId { get; }
        public int Offset { get; }
    }
}