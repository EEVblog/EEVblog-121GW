using rMultiplatform;
using rMultiplatform.BLE;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace App_112GW
{
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        private Settings            SettingsView    = new Settings();
        private MathChartSettings   MathChart       = new MathChartSettings();

        private void SettingsView_AddDevice(IDeviceBLE pDevice)
        {
            var dev = new Multimeter(pDevice);
            Devices.Add(dev);
            Children.Add(dev);
        }

        private void SettingsView_RemoveDevices()
        {
            for ( var i = 0; i < Children.Count; ++i)
            {
                var item = Children[i];
                if (item.GetType() == typeof(Multimeter))
                {
                    var meter = item as Multimeter;
                    meter.mDevice.Unregister();
                    Children.Remove(item);
                }
            }
            SettingsView.RemoveDevice();
        }

        ObservableCollection<Multimeter> Devices = new ObservableCollection<Multimeter>();
        public MainPage()
        {
            CappedRange temp = new CappedRange(0.0, 10.0);
            temp.Zoom(2, 6);
            temp.Pan(1);
            BackgroundColor = Globals.BackgroundColor;
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            Children.Add(SettingsView);
            SettingsView.AddDevice += SettingsView_AddDevice;
            SettingsView.RemoveDevices += SettingsView_RemoveDevices;
            Padding = 10;

            Children.Add( new ContentPage()
            {
                Title = "< Maths >",
                Content = MathChart
            });

            MathChart.SourceA = Devices;
            MathChart.SourceB = Devices;
        }
    }
}
