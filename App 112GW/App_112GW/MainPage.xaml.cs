using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using rMultiplatform;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
	public partial class MainPage : ContentPage
	{
        public List<MultimeterThemed> Devices = new List<MultimeterThemed>();
        public List<Chart> Charts = new List<Chart>();

        private Button          ButtonAddDevice		= new Button        { Text = "Add Device"      };
		private Button		    ButtonStartLogging	= new Button        { Text = "Start Logging"   };
		private Grid		    UserGrid			= new Grid          { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1};
        private ScrollView      DeviceView          = new ScrollView    { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.Fill };
        private StackLayout     DeviceLayout        = new StackLayout   { HorizontalOptions = LayoutOptions.Fill,   VerticalOptions = LayoutOptions.StartAndExpand };

        void InitSurface()
		{
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(1,     GridUnitType.Star)      });
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(50,    GridUnitType.Absolute)	});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1,     GridUnitType.Star)		});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1,     GridUnitType.Star)		});

            DeviceView.Content = DeviceLayout;
            UserGrid.Children.Add	(DeviceView);
			Grid.SetRow				(DeviceView, 0);
			Grid.SetColumn			(DeviceView, 0);
			Grid.SetRowSpan			(DeviceView, 1);
			Grid.SetColumnSpan		(DeviceView, 2);

			UserGrid.Children.Add	(ButtonAddDevice,		0, 1);
			UserGrid.Children.Add	(ButtonStartLogging,	1, 1);
			Grid.SetColumnSpan		(ButtonAddDevice,		1);
			Grid.SetColumnSpan		(ButtonStartLogging,	1);
			
			ButtonAddDevice.Clicked		+= AddDevice;
			ButtonStartLogging.Clicked	+= StartLogging;

			Content = UserGrid;
		}
		public MainPage ()
		{
			InitializeComponent();
			InitSurface();
		}

        ChartData Data0 = new ChartData(ChartData.ChartDataMode.eRolling,   "Time (s)", "Volts (V)",    0.1f,     10.0f);
        ChartData Data1 = new ChartData(ChartData.ChartDataMode.eRolling,   "Time (s)", "Volts (V)",    0.1f,     10.0f);
        ChartData Data2 = new ChartData(ChartData.ChartDataMode.eRolling,   "Time (s)", "Volts (V)",    0.1f,     10.0f);

        void AddDevice (object sender, EventArgs args)
		{
            var Temp1 = new MultimeterThemed(Globals.BackgroundColor);
            Devices.Add(Temp1);
            DeviceLayout.Children.Add(Temp1);
            Grid.SetRow(Temp1, 0);
            Grid.SetColumn(Temp1, 0);
            Grid.SetRowSpan(Temp1, 1);
            Grid.SetColumnSpan(Temp1, 2);

            var Temp2 = new Chart() { Padding = new rMultiplatform.ChartPadding(0.1) };
            Temp2.AddGrid(new ChartGrid());
            Temp2.AddAxis(new ChartAxis(5, 5, 0, 20)    {Label = "Time (s)",   Orientation = ChartAxis.AxisOrientation.Horizontal, AxisLocation = 0.9, LockToAxisLabel = "Volts (V)",   LockAlignment = ChartAxis.AxisLock.eEnd     });
            Temp2.AddAxis(new ChartAxis(5, 5, 0, 0)     {Label = "Volts (V)",  Orientation = ChartAxis.AxisOrientation.Vertical,   AxisLocation = 0.1, LockToAxisLabel = "Time (s)" ,   LockAlignment = ChartAxis.AxisLock.eStart   });

            Temp2.AddData(Data0);
            Temp2.AddData(Data1);
            Temp2.AddData(Data2);

            Charts.Add(Temp2);
            DeviceLayout.Children.Add(Temp2);
            Grid.SetRow(Temp2, 0);
            Grid.SetColumn(Temp2, 0);
            Grid.SetRowSpan(Temp2, 1);
            Grid.SetColumnSpan(Temp2, 2);

            UserGrid.Children.Add       (ButtonAddDevice,       0, 1);
            UserGrid.Children.Add       (ButtonStartLogging,    1, 1);
            Grid.SetColumnSpan          (ButtonAddDevice,       1);
            Grid.SetColumnSpan          (ButtonStartLogging,    1);
        }

        bool rtn = false;
        int vals = 0;
        bool UpdateValue()
        {
            vals += 1;
            if (vals >= 99999) vals = 0;

            foreach (MultimeterThemed temp in Devices)
            {
                temp.Screen.LargeSegments = (float)vals;
                temp.Screen.SmallSegments = 99999 - vals;
                temp.Screen.Bargraph = (vals % 20);
            }

            Data0.Sample(Globals.RandBetween(1, 2));
            Data1.Sample(Globals.RandBetween(0, 1));
            Data2.Sample(Globals.RandBetween(-1, 0));
            return rtn;
        }

        void StartLogging (object sender, EventArgs args)
        {
            if (rtn)
            {
                rtn = false;
            }
            else
            {
                rtn = true;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), UpdateValue);
            }

            UpdateValue();
        }
	}
}
