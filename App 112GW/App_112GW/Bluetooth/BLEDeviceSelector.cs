using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace rMultiplatform.BLE
{
	public class BLEDeviceSelector : ContentView
	{
        List<IDeviceBLE> mDevices;

		public BLEDeviceSelector ()
		{
			Content = new StackLayout {
				Children = {
					new Label { Text = "Welcome to Xamarin Forms!" }
				}
			};
		}
	}
}