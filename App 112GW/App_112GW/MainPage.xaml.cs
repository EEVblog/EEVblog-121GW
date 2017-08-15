using rMultiplatform;
using rMultiplatform.BLE;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace App_112GW
{
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        private Settings SettingsView = new Settings();
        
        private void SettingsView_AddDevice(IDeviceBLE pDevice)
        {
            Children.Add(new Multimeter(pDevice));
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
            BackgroundColor = Globals.BackgroundColor;
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            Children.Add(SettingsView);
            SettingsView.AddDevice += SettingsView_AddDevice;
            SettingsView.RemoveDevices += SettingsView_RemoveDevices;
            Padding = 10;


            Children.Add(
                new ContentPage()
                {
                    Title = "This",
                    Content = new MathChartSettings()
                });
        }
    }
}
