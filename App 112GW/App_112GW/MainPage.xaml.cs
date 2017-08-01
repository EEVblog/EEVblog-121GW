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
using rMultiplatform.BLE;

namespace App_112GW
{
	public partial class MainPage : ContentPage
	{
        const double DefaultWidth = 500;


        public List<Multimeter> Devices = new List<Multimeter>();
        public BLEDeviceSelector BLESelectDevice = new BLEDeviceSelector();
        private Button          ButtonAddDevice		= new Button        { Text = "Add Device"      };
		private Button		    ButtonStartLogging	= new Button        { Text = "Start Logging"   };
		private Grid		    UserGrid			= new Grid          { HorizontalOptions = LayoutOptions.CenterAndExpand,   VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1};
        private ScrollView      DeviceView          = new ScrollView    { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.Fill };
        private StackLayout     DeviceLayout        = new StackLayout   { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.StartAndExpand };

        void    InitSurface()
        {
            //Setup connected event
            BackgroundColor             = Globals.BackgroundColor;
            UserGrid.BackgroundColor    = Globals.BackgroundColor;

            UserGrid.RowDefinitions.Add     (	new RowDefinition   { Height    = new GridLength(1,     GridUnitType.Star)      });
			UserGrid.RowDefinitions.Add     (	new RowDefinition	{ Height	= new GridLength(50,    GridUnitType.Absolute)	});
			UserGrid.ColumnDefinitions.Add  (	new ColumnDefinition{ Width		= new GridLength(1,     GridUnitType.Star)		});
			UserGrid.ColumnDefinitions.Add  (	new ColumnDefinition{ Width		= new GridLength(1,     GridUnitType.Star)		});

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
            ButtonAddDevice.Clicked	+= SelectDevice;
            UserGrid.WidthRequest = DefaultWidth;

            BLESelectDevice = new BLEDeviceSelector();
            BLESelectDevice.Connected += Connected;

            //Content = BLESelectDevice;
            Content = UserGrid;
        }
        public  MainPage ()
		{
			InitializeComponent();
			InitSurface();
        }
        void    SelectDevice (object o, EventArgs e)
        {
            //Production Code
            //BLESelectDevice.Reset();
            //Content = BLESelectDevice;

            //Mock Code
            AddDevice(null);
        }

        //Only maintains aspect ratio
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }
        void    AddDevice (IDeviceBLE pDevice)
		{
            Debug.WriteLine("ADDING DEVICE.");
            var NewDevice = new Multimeter(pDevice);
            NewDevice.RequestMaximise   += NewDevice_RequestMaximise;
            NewDevice.RequestRestore    += NewDevice_RequestRestore;
            Devices.Add(NewDevice);
            DeviceLayout.Children.Add(NewDevice);
            Grid.SetRow(NewDevice, 0);
            Grid.SetColumn(NewDevice, 0);
            Grid.SetRowSpan(NewDevice, 1);
            Grid.SetColumnSpan(NewDevice, 2);
        }

        private void NewDevice_RequestRestore(object sender, EventArgs e)
        {
            foreach (var multimeter in Devices)
                multimeter.RestoreVisibleState();

            UserGrid.WidthRequest = DefaultWidth;
            InvalidateMeasure();
        }

        private void NewDevice_RequestMaximise(object sender, EventArgs e)
        {
            foreach (var multimeter in Devices)
                multimeter.HideVisibleState();

            UserGrid.WidthRequest = Width;
            InvalidateMeasure();
        
            (sender as Multimeter).IsVisible = true;
        }

        void    Connected(IDeviceBLE pDevice)
        {
            //BLESelectDevice = null;
            if (pDevice == null)
                return;
            Debug.WriteLine("Connected to device : " + pDevice.Name);

            //Add multimeter
            AddDevice(pDevice);
            Device.BeginInvokeOnMainThread( () =>
            {
                Content = UserGrid;
            });
        }
    }
}
