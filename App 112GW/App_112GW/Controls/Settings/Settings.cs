using App_112GW;
using rMultiplatform.BLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace rMultiplatform
{
	public class Settings : ContentPage
	{
        private BLEDeviceSelector BLESelectDevice;

        private Grid UserGrid       = new Grid  { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, RowSpacing = 1, ColumnSpacing = 1, Padding = 1 };
        private Button ButtonLeft   = new Button();
        private Button ButtonRight  = new Button();

        public Settings ()
        {
            Title = "< Settings >";

            //Setup connected event
            UserGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            UserGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            UserGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });
            UserGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            UserGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //
            UserGrid.Children.Add(ButtonLeft, 0, 2);
            UserGrid.Children.Add(ButtonRight, 1, 2);
            Grid.SetColumnSpan(ButtonLeft, 1);
            Grid.SetColumnSpan(ButtonRight, 1);

            //
            ButtonLeft.Clicked += ButtonLeft_Clicked;
            ButtonRight.Clicked += ButtonRight_Clicked;

            //
            BLESelectDevice = new BLEDeviceSelector();
            BLESelectDevice.Connected += Connected;

            UserGrid.Children.Add(BLESelectDevice, 0, 0);
            Grid.SetColumnSpan(BLESelectDevice, 2);
            Grid.SetRowSpan(BLESelectDevice, 1);

            ClearRightButton();
            ClearLeftButton();

            SetRightButton("Remove Devices", (o, e) => { RemoveDevices?.Invoke(); });
            SetLeftButton("Refresh", RefreshDevices);
            Content = UserGrid;
        }

        private void RefreshDevices(object sender, EventArgs e)
        {
            BLESelectDevice.mClient.Reset();
        }

        public void RemoveDevice()
        {
            BLESelectDevice.RemoveDevices();
        }

        public delegate void AddBluetoothDevice(IDeviceBLE pDevice);
        public event AddBluetoothDevice AddDevice;

        public delegate void BasicFunction();
        public event BasicFunction RemoveDevices;
        
        private void Connected(IDeviceBLE pDevice)
        {
            if (pDevice == null)
                return;
            Debug.WriteLine("Connected to device : " + pDevice.Name);
            AddDevice?.Invoke(pDevice);
        }

        private event EventHandler _LeftButtonEvent;
        private event EventHandler _RightButtonEvent;
        private event EventHandler LeftButtonEvent
        {
            add
            {
                _LeftButtonEvent += value;
            }
            remove
            {
                _LeftButtonEvent -= value;
            }
        }
        private event EventHandler RightButtonEvent
        {
            add
            {
                _RightButtonEvent += value;
            }
            remove
            {
                _RightButtonEvent -= value;
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
        private void NoAction(object o, EventArgs e) { }

        private string LeftButtonText
        {
            set
            {
                ButtonLeft.Text = value;
            }
        }
        private string RightButtonText
        {
            set
            {
                ButtonRight.Text = value;
            }
        }
        public void SetLeftButton(string LeftText, EventHandler LeftEvent)
        {
            _LeftButtonEvent = null;
            LeftButtonEvent += LeftEvent;
            LeftButtonText = LeftText;
            ButtonLeft.IsVisible = true;
        }
        public void SetRightButton(string RightText, EventHandler RightEvent)
        {
            _RightButtonEvent = null;
            RightButtonEvent += RightEvent;
            RightButtonText = RightText;
            ButtonRight.IsVisible = true;
        }
        public void ClearRightButton()
        {
            _RightButtonEvent = null;
            RightButtonEvent += NoAction;
            ButtonRight.IsVisible = false;
        }
        public void ClearLeftButton()
        {
            _LeftButtonEvent = null;
            LeftButtonEvent += NoAction;
            ButtonLeft.IsVisible = false;
        }
    }
}