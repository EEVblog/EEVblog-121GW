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
        PacketProcessor MyProcessor = new PacketProcessor(0xF2, 26);
        public MainPage ()
		{
			InitializeComponent();
			InitSurface();

            MyProcessor.mCallback += ProcessPacket;
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
            
            return true;
        }

        IClientBLE client = new ClientBLE ();
        IDeviceBLE device = null;

        bool loop = true;
        async void AsyncStartLogging (object sender, EventArgs args)
        { 
            await Task.Run(() =>
            {
                //Wait for device to appear
                while (loop)
                {
                    foreach (var line in client.ListDevices())
                    {
                        if (line.Name.Contains("CR"))
                        {
                            device = line;

                            //Setup service events
                            loop = false;
                            break;
                        }
                    }
                }

                var services = client.Connect(device).Services;
                foreach (var serv in services)
                    foreach (var chari in serv.Characteristics)
                        if (chari.Description.Length > 0)
                        {
                            Debug.WriteLine("Setting up event for : " + chari.Description);
                            chari.ValueChanged += MainPage_ValueChanged;
                        }
            });
        }
        void StartLogging (object sender, EventArgs args)
        {
            AsyncStartLogging(sender, args);
        }

        
        void ProcessPacket(byte[] pInput)
        {
            var processor = new rMultiplatform.Packet112GW();
            try
            {
                processor.ProcessPacket(pInput);

                if (Devices.Count == 0)
                    return;

                var dev = Devices.Last();
                //dev.Screen.LargeSegments    = processor.MainValue;
                //dev.Screen.SmallSegments    = processor.SubValue;
                //dev.Screen.Bargraph         = processor.BarValue;
                dev.Data.Sample(processor.MainValue);

                dev.Screen.Update(processor);
                dev.Screen.InvalidateSurface();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

       
        private void MainPage_ValueChanged(object o, rMultiplatform.BLE.CharacteristicEvent v)
        {
            Debug.WriteLine("Recieved: " + v.NewValue);
            MyProcessor.Recieve(v.Bytes);
        }
    }
}
