using rMultiplatform;
using rMultiplatform.BLE;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace App_112GW
{
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        private ObservableCollection<Multimeter> Devices = new ObservableCollection<Multimeter>();
        private Settings            SettingsView    = new Settings();
        private MathChart MathChart = new MathChart();

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

        public MainPage()
        {
            Padding = 0;
            BackgroundColor = Globals.BackgroundColor;

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            
            Children.Add(SettingsView);
            SettingsView.AddDevice += SettingsView_AddDevice;
            SettingsView.RemoveDevices += SettingsView_RemoveDevices;

            Children.Add( new ContentPage()
            {
                Title = "< Maths >",
                Content = MathChart,
            });
            MathChart.SourceA = Devices;
            MathChart.SourceB = Devices;
        }
    }
}