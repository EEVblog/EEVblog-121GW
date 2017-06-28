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

        ListView listView;
        void UpdateDeviceList()
        {
            listView.ItemsSource = null;
            listView.ItemsSource = mClient.ListDevices();
        }

        public void Reset()
        {
            mClient.Reset();
        }

        IClientBLE mClient;
        public BLEDeviceSelector()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;

            listView = new ListView();
            listView.BackgroundColor = Globals.BackgroundColor;

            //Reset BLE
            mClient = null;
            mClient = new ClientBLE();
            mClient.DeviceListUpdated += UpdateDeviceList;
            Reset();

            //
            var template = new DataTemplate(typeof(TextCell));

            // We can set data bindings to our supplied objects.
            template.SetBinding(TextCell.TextProperty, "Name");
            template.SetBinding(TextCell.DetailProperty, "Id");
            template.SetValue(TextCell.TextColorProperty, Globals.TextColor);
            template.SetValue(TextCell.DetailColorProperty, Globals.HighlightColor);

            //
            listView.ItemTemplate = template;
            listView.ItemSelected += OnSelection;
            Content = listView;
        }

        private void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as IDeviceBLE;
            var dev = Connect(item);
            Connected?.Invoke(dev);
        }

        private IDeviceBLE Connect(IDeviceBLE Device)
        {
            if (Device == null)
                return null;

            //Wait for device to appear
            if (mClient != null)
            {
                var rtn = mClient.Connect(Device);
                if ( rtn == null )
                {
                    listView.SelectedItem = null;
                    return null;
                }
                else
                    return rtn;
            }
            return null;
        }
    }
}