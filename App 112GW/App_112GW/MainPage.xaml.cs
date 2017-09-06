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
        private Settings SettingsView;
        private MathChart MathChart;

        private void AddDevice(Multimeter Device)
        {
            Devices.Add(Device);
            if (Devices.Count == 1)
                AddPage("< Maths >", MathChart);
        }
        private void AddPage(string Title, View Content)
        {
            Children.Add(new GeneralPage(Title, Content));
        }
        private void Button_AddDevice(IDeviceBLE pDevice)
        {
            var dev = new Multimeter(pDevice);
            AddDevice(dev);
            AddPage("[" + dev.ShortId + "]", dev);
        }

        public MainPage()
        {
            BackgroundColor = Globals.BackgroundColor;
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);

            MathChart = new MathChart();
            MathChart.SourceA = Devices;
            MathChart.SourceB = Devices;

            SettingsView = new Settings();
            SettingsView.AddDevice += Button_AddDevice;

            AddPage("< Settings >", SettingsView);
        }
    }
}