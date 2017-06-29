﻿using System;
using System.Reactive.Linq;


namespace Plugin.BluetoothLE
{
    public class AdapterScanner : IAdapterScanner
    {
        public bool IsSupported => false;
        public IObservable<IAdapter> FindAdapters() => Observable.Empty<IAdapter>();
    }
}
