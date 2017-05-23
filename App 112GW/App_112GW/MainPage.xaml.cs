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
		Random              randy               = new Random();
        private Button      ButtonAddDevice		= new Button {  Text = "Add Device" };
		private Button		ButtonStartLogging	= new Button {  Text = "Start Logging" };
		private Grid		UserGrid			= new Grid   {  HorizontalOptions=LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1};

        List<MultimeterThemed>    Devices       = new List<MultimeterThemed>();
        private ScrollView  DeviceView          = new ScrollView { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        private StackLayout DeviceLayout        = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.StartAndExpand };
		void InitSurface()
		{
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(1, GridUnitType.Star)});
			UserGrid.RowDefinitions.Add(	new RowDefinition		{ Height	= new GridLength(50, GridUnitType.Absolute)	});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1, GridUnitType.Star)		});
			UserGrid.ColumnDefinitions.Add(	new ColumnDefinition	{ Width		= new GridLength(1, GridUnitType.Star)		});

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

		void AddDevice (object sender, EventArgs args)
		{
            MultimeterThemed Temp = new MultimeterThemed(Color.White, Color.Black);

            Devices.Add(Temp);
            DeviceLayout.Children.Add   (Temp);
            Grid.SetRow                 (Temp, 0);
            Grid.SetColumn              (Temp, 0);
            Grid.SetRowSpan             (Temp, 1);
            Grid.SetColumnSpan          (Temp, 2);

            UserGrid.Children.Add       (ButtonAddDevice,       0, 1);
            UserGrid.Children.Add       (ButtonStartLogging,    1, 1);
            Grid.SetColumnSpan          (ButtonAddDevice,       1);
            Grid.SetColumnSpan          (ButtonStartLogging,    1);
        }

        void StartLogging(object sender, EventArgs args)
        {
            Devices.Last().Clicked(sender, args);
            Devices.Last().Style.BasedOn = UserGrid.Style;
        }
	}
}
