using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using rMultiplatform;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;

namespace App_112GW
{
	public partial class MainPage : ContentPage
	{
        public List<rMultiplatform.MultimeterThemed> Devices = new List<rMultiplatform.MultimeterThemed>();

        private Button          ButtonAddDevice		= new Button        { Text = "Add Device"      };
		private Button		    ButtonStartLogging	= new Button        { Text = "Start Logging"   };
		private Grid		    UserGrid			= new Grid          { HorizontalOptions = LayoutOptions.CenterAndExpand,   VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1};
        private ScrollView      DeviceView          = new ScrollView    { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.Fill };
        private StackLayout     DeviceLayout        = new StackLayout   { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.StartAndExpand };

        void InitSurface()
		{
            BackgroundColor             = App_112GW.Globals.BackgroundColor;
            UserGrid.BackgroundColor    = App_112GW.Globals.BackgroundColor;

            UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(1,     GridUnitType.Star)      });
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(50,    GridUnitType.Absolute)	});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1,     GridUnitType.Star)		});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1,     GridUnitType.Star)		});

            //
            DeviceView.Content = DeviceLayout;
            UserGrid.Children.Add	(DeviceView);

            //
			Grid.SetRow				(DeviceView, 0);
			Grid.SetColumn			(DeviceView, 0);
			Grid.SetRowSpan			(DeviceView, 1);
			Grid.SetColumnSpan		(DeviceView, 2);

            //
			UserGrid.Children.Add	(ButtonAddDevice,		0, 1);
			UserGrid.Children.Add	(ButtonStartLogging,	1, 1);
			Grid.SetColumnSpan		(ButtonAddDevice,		1);
			Grid.SetColumnSpan		(ButtonStartLogging,	1);
			
            //
			ButtonAddDevice.Clicked		+= AddDevice;
			ButtonStartLogging.Clicked	+= StartLogging;
            
            UserGrid.WidthRequest = 400;
            Content = UserGrid;
		}

        public MainPage ()
		{
			InitializeComponent();
			InitSurface();
        }

        //Only maintains aspect ratio
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }


        void AddDevice (object sender, EventArgs args)
		{
            var Temp1 = new rMultiplatform.MultimeterThemed(Globals.BackgroundColor);
            Devices.Add(Temp1);
            DeviceLayout.Children.Add(Temp1);
            Grid.SetRow(Temp1, 0);
            Grid.SetColumn(Temp1, 0);
            Grid.SetRowSpan(Temp1, 1);
            Grid.SetColumnSpan(Temp1, 2);

            UserGrid.Children.Add       (ButtonAddDevice,       0, 1);
            UserGrid.Children.Add       (ButtonStartLogging,    1, 1);
            Grid.SetColumnSpan          (ButtonAddDevice,       1);
            Grid.SetColumnSpan          (ButtonStartLogging,    1);
        }

        bool UpdateValue(float value)
        {
            if (Devices.Count == 0)
                return false;

            var dev = Devices.Last();
            Devices.Last().Screen.LargeSegments = (float)value;
            dev.Data.Sample(value);

            dev.Screen.InvalidateSurface();
            return true;
        }

        rMultiplatform.BLE.IDeviceBLE device = null;
        rMultiplatform.BLE.IClientBLE temp = new rMultiplatform.BLE.ClientBLE();
        List<rMultiplatform.BLE.IDeviceBLE> devices = new List<rMultiplatform.BLE.IDeviceBLE>();

        bool lockme = false;
        async void AsyncStartLogging(object sender, EventArgs args)
        {
            if (lockme)
                return;
            lockme = true;

            devices = temp.ListDevices();
            foreach (var line in devices)
                if (line.name.Contains("CR"))
                {
                    device = (await temp.Connect(line));
                    Debug.WriteLine("Found Device: " + device.name);
                }

            if (device != null)
            {
                Debug.WriteLine("Current Device: " + device.name);
                foreach (var serv in device.Services)
                    foreach (var chari in serv.Characteristics)
                        chari.ValueChanged += MainPage_ValueChanged;
            }
            else
                Debug.WriteLine("No device added.");


            lockme = false;
        }
        void StartLogging (object sender, EventArgs args)
        {
            AsyncStartLogging(sender, args);
        }

        bool next = false;
        private void MainPage_ValueChanged(object o, rMultiplatform.BLE.CharacteristicEvent v)
        {
            Debug.WriteLine("Recieved: " + v.NewValue);

            var first = (byte)v.Bytes[0];
            if (first == 0xf2)
                next = true;
            else
            if (next)
            {
                var valuehexMSB = Convert.ToInt64(v.NewValue.Substring(4, 2), 16);
                var valuehexLSB = Convert.ToInt64(v.NewValue.Substring(6, 2), 16);

                var result = (float)(valuehexMSB << 8 | valuehexLSB) / 10;

                UpdateValue(result);
                next = false;
            }
        }
    }
}
