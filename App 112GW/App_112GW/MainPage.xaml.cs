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
		private Settings SettingsView = new Settings();
        private MathChart MathChart = new MathChart();

        private void AddPage(string Title, View Content)
        {
            Children.Add(new GeneralPage(Title, Content));
        }
        private void AddDevice(MultimeterPage Device)
		{
            MathChart.AddDevice(Device);
			if (MathChart.Devices.Count == 1)
				AddPage("< Maths >", MathChart);
        }
		private void Button_AddDevice(IDeviceBLE pDevice)
		{
			var dev = new MultimeterPage(pDevice);

            AddDevice(dev);
            Children.Add(dev);
            CurrentPage = Children[Children.Count - 1];
        }

		public MainPage()
		{
			BackgroundColor = Globals.BackgroundColor;
			On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
			SettingsView.AddDevice += Button_AddDevice;

            AddPage("< Settings >", SettingsView);
		}
    }
}