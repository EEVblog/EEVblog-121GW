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
            try
            {
                if (listView.ItemsSource == null)
                    listView.ItemsSource = mClient.ListDevices();
            }
            catch
            {
                Debug.WriteLine("Error Caught : void UpdateDeviceList()");
            }
        }
        public void Reset()
        {
            try
            {
                mClient.Reset();
            }
            catch
            {
                Debug.WriteLine("Error Caught : public void Reset()");
            }
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
            mClient.DeviceConnected += MClient_DeviceConnected;
           // Reset();

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

        List<Object> devucesl = new List<object>();
        private void MClient_DeviceConnected(IDeviceBLE pDevice)
        {
            try
            {
                Debug.WriteLine("Error Caught : 1");
                listView.SelectedItem = null;
                Debug.WriteLine("Error Caught : 2");
                Connected?.Invoke(pDevice);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Caught : private void MClient_DeviceConnected(IDeviceBLE pDevice)");
            }
        }
        private void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as IDeviceBLE;
            Connect(item);
        }

        private void Connect (IDeviceBLE Device)
        {
            if (Device == null)
                return;

            //Wait for device to appear
            if (mClient != null)
                mClient.Connect(Device);
            mClient.Stop();
        }
    }
}