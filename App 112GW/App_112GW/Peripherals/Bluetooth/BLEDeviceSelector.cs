using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using App_112GW;

namespace rMultiplatform.BLE
{
    public class BLEDeviceSelector : ContentView
    {
        public delegate void DeviceConnected(IDeviceBLE pDevice);
        public event DeviceConnected Connected;

        IClientBLE mClient;
        public BLEDeviceSelector()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;

            mClient = new ClientBLE();

            //
            var listView = new ListView();
            listView.ItemsSource = mClient.ListDevices();

            //
            listView.BackgroundColor = Globals.BackgroundColor;

            var template = new DataTemplate(typeof(TextCell));

            // We can set data bindings to our supplied objects.
            template.SetBinding(TextCell.TextProperty, "Name");
            template.SetBinding(TextCell.DetailProperty, "Id");
            template.SetValue(TextCell.TextColorProperty, Globals.TextColor);
            template.SetValue(TextCell.DetailColorProperty, Globals.HighlightColor);


            listView.ItemTemplate = template;
            listView.ItemSelected += OnSelection;
            Content = listView;
        }

        private void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as IDeviceBLE;
            Task.Run(() =>
            {
                var dev = Connect(item);
                Connected?.Invoke(dev);
            });
        }

        private IDeviceBLE Connect(IDeviceBLE Device)
        {
            //Wait for device to appear
            if (mClient != null)
            {
                return mClient.Connect(Device);
            }
            return null;
        }
    }
}