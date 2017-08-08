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
        private Button ButtonLeft   = new Button { Text = "Add Device" };
        private Button ButtonRight  = new Button { Text = "Start Logging" };
        private Grid UserGrid = new Grid { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1 };
        private ScrollView Scrollable = new ScrollView { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        private StackLayout DeviceLayout = new StackLayout { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.StartAndExpand };

        void InitSurface()
        {
            //Setup connected event
            BackgroundColor = Color.Black ;
            UserGrid.BackgroundColor = Globals.BackgroundColor;
            UserGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            UserGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });
            UserGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            UserGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //
            Scrollable.Content = DeviceLayout;
            UserGrid.Children.Add(Scrollable);

            //
            Grid.SetRow         (Scrollable, 0);
            Grid.SetColumn      (Scrollable, 0);
            Grid.SetRowSpan     (Scrollable, 1);
            Grid.SetColumnSpan  (Scrollable, 2);

            //
            UserGrid.Children.Add   (ButtonLeft,    0,  1);
            UserGrid.Children.Add   (ButtonRight,   1,  1);
            Grid.SetColumnSpan      (ButtonLeft,    1);
            Grid.SetColumnSpan      (ButtonRight,   1);

            //
            ButtonLeft.Clicked      +=  ButtonLeft_Clicked;
            ButtonRight.Clicked     +=  ButtonRight_Clicked;
            UserGrid.WidthRequest   =   DefaultWidth;

            BLESelectDevice = new BLEDeviceSelector();
            BLESelectDevice.Connected += Connected;
            SetView(CurrentView.Add);

            Content = UserGrid;
        }
        
        public string LeftButtonText
        {
            set
            {
                ButtonLeft.Text = value;
            }
        }
        public string RightButtonText
        {
            set
            {
                ButtonRight.Text = value;
            }
        }
        private EventHandler _LeftButtonEvent;
        private EventHandler _RightButtonEvent;
        private EventHandler LeftButtonEvent
        {
            set
            {
                _LeftButtonEvent = value;
            }
        }
        private EventHandler RightButtonEvent
        {
            set
            {
                _RightButtonEvent = value;
            }
        }

        private void ButtonRight_Clicked(object sender, EventArgs e)
        {
            _RightButtonEvent?.Invoke(sender, e);
        }
        private void ButtonLeft_Clicked(object sender, EventArgs e)
        {
            _LeftButtonEvent?.Invoke(sender, e);
        }

        void SetLeftButton(string LeftText, EventHandler LeftEvent)
        {
            LeftButtonEvent     = LeftEvent;
            LeftButtonText      = LeftText;
            ButtonLeft.IsVisible = true;
        }
        void SetRightButton(string RightText, EventHandler RightEvent)
        {
            RightButtonEvent    = RightEvent;
            RightButtonText     = RightText;
            ButtonRight.IsVisible = true;
        }
        void ClearRightButton()
        {
            RightButtonEvent = NoAction;
            ButtonRight.IsVisible = false;
        }
        void ClearLeftButton()
        {
            LeftButtonEvent = NoAction;
            ButtonLeft.IsVisible = false;
        }

        enum CurrentView
        {
            Current,
            Add,
            Remove
        }
        void SetContent(View Content)
        {
            Scrollable.Content = Content;
        }
        void SetView(CurrentView View)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (View)
                {
                    case CurrentView.Current:
                        SetContent(DeviceLayout);
                        SetLeftButton("Add Device", AddDevice);
                        ClearRightButton();
                        break;
                    case CurrentView.Add:
                        BLESelectDevice.Reset();
                        SetContent(BLESelectDevice);
                        SetRightButton("Cancel", CancelAddDevice);
                        ClearLeftButton();
                        break;
                }
            });
        }

        public MainPage()
		{
			InitializeComponent();
			InitSurface();
        }
        void AddDevice(IDeviceBLE pDevice)
		{
            var NewDevice = new Multimeter(pDevice);
            NewDevice.RequestMaximise   += NewDevice_RequestMaximise;
            NewDevice.RequestRestore    += NewDevice_RequestRestore;
            Devices.Add(NewDevice);
            DeviceLayout.Children.Add(NewDevice);
        }
        private void NewDevice_RequestRestore(object sender, EventArgs e)
        {
            foreach (var multimeter in Devices)
                multimeter.IsVisible = true;

            UserGrid.WidthRequest = DefaultWidth;
            InvalidateMeasure();
        }
        private void NewDevice_RequestMaximise(object sender, EventArgs e)
        {
            var current = (sender as Multimeter);
            foreach (var multimeter in Devices)
                multimeter.IsVisible = (current.Equals(multimeter));

            UserGrid.WidthRequest = Width;
            InvalidateMeasure();
        }
        private void Connected(IDeviceBLE pDevice)
        {
            if (pDevice == null)
                return;
            Debug.WriteLine("Connected to device : " + pDevice.Name);

            //Add multimeter
            AddDevice(pDevice);
            SetView(CurrentView.Current);
        }
        private void CancelAddDevice(object sender, EventArgs e)
        {
            SetView(CurrentView.Current);
        }
        private void AddDevice(object o, EventArgs e)
        {
            SetView(CurrentView.Add);
        }
        private void NoAction(object o, EventArgs e){}
    }
}
