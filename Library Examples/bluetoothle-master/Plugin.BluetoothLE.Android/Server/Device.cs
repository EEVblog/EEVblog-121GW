﻿using System;
using System.Linq;
using Android.Bluetooth;


namespace Plugin.BluetoothLE.Server
{
    public class Device : IDevice
    {
        readonly Lazy<Guid> deviceUuidLazy;


        public Device(BluetoothDevice native)
        {
            this.Native = native;
            this.deviceUuidLazy = new Lazy<Guid>(() =>
            {
                var deviceGuid = new byte[16];
                var mac = native.Address.Replace(":", "");
                var macBytes = Enumerable
                    .Range(0, mac.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(mac.Substring(x, 2), 16))
                    .ToArray();

                macBytes.CopyTo(deviceGuid, 10);
                return new Guid(deviceGuid);
            });
        }


        public BluetoothDevice Native { get; }
        public Guid Uuid => this.deviceUuidLazy.Value;
        public object Context { get; set; }
    }
}
