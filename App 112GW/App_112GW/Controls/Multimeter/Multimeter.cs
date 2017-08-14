using System;
using System.Threading;
using App_112GW;
using Xamarin.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
    public partial class Multimeter : ContentPage
    {
        public event EventHandler RequestMaximise;
        public event EventHandler RequestRestore;

        public BLE.IDeviceBLE mDevice;
        PacketProcessor MyProcessor = new PacketProcessor(0xF2, 26);
        void ProcessPacket(byte[] pInput)
        {
            var processor = new Packet112GW();
            try
            {
                processor.ProcessPacket(pInput);
                Data.Sample(processor.MainValue);

                Screen.Update(processor);
                Screen.InvalidateSurface();
            }
            catch (Exception ex)
            {
                MyProcessor.Reset();
                Debug.WriteLine(ex.ToString());
            }
        }

        public new string Id
        {
            get
            {
                return mDevice.Id;
            }
        }

        Timer stateTimer;

        public Grid                     MultimeterGrid;
        public MultimeterScreen         Screen;
        public MultimeterMenu           Menu;
        public ChartData                Data;
        public Chart                    Plot;

        enum ActiveItem
        {
            Screen,
            Menu,
            FullscreenPlot
        }
        ActiveItem Item = ActiveItem.Screen;
        Random RandomGen = new Random();
        private void TestCallback(object state)
        {
            Data.Sample((RandomGen.NextDouble() - 0.5) * 2.0);
        }

        public Multimeter ( BLE.IDeviceBLE pDevice )
        {
            Padding = 20;

            if (pDevice == null)
                stateTimer = new Timer(TestCallback, null, 1000, 1000);
            else
            {
                mDevice = pDevice;
                mDevice.Change += ValueChanged;
                MyProcessor.mCallback += ProcessPacket;
            }

            Screen                  =   new MultimeterScreen();
            Screen.Clicked          +=  Menu_BackClicked;

            string id;
            if (pDevice == null)
                Menu = new MultimeterMenu();
            else
            {
                id = pDevice.Id;
                Title = "[ " + id.Substring(id.Length - 5) + " ]";
                Menu = new MultimeterMenu(id.Substring(id.Length - 5));
            }

            Menu.BackClicked        +=  Menu_BackClicked;
            Menu.HoldClicked        +=  Menu_HoldClicked;
            Menu.RelClicked         +=  Menu_RelClicked;
            Menu.ModeChanged        +=  Menu_ModeChanged;
            Menu.RangeChanged       +=  Menu_RangeChanged;
            
            Data = new ChartData( ChartData.ChartDataMode.eRescaling, "Time (s)", "Volts (V)", 10f );
            Plot = new Chart() { Padding = new ChartPadding( 0.1f ) };
            Plot.AddGrid(new ChartGrid());
            Plot.AddAxis(new ChartAxis(5, 5, 0, 20) {   Label = "Time (s)", Orientation = ChartAxis.AxisOrientation.Horizontal, LockToAxisLabel = "Volts (V)",  LockAlignment = ChartAxis.AxisLock.eEnd, ShowDataKey = false });
            Plot.AddAxis(new ChartAxis(5, 5, 0, 0)  {   Label = "Volts (V)",Orientation = ChartAxis.AxisOrientation.Vertical,   LockToAxisLabel = "Time (s)",   LockAlignment = ChartAxis.AxisLock.eStart});
            Plot.AddData(Data);

            MultimeterGrid = new Grid();
            MultimeterGrid.RowDefinitions.Add       (   new RowDefinition      { Height    = new GridLength(1, GridUnitType.Auto)  });
            MultimeterGrid.RowDefinitions.Add       (   new RowDefinition      { Height    = new GridLength(1, GridUnitType.Star)  });
            MultimeterGrid.ColumnDefinitions.Add    (   new ColumnDefinition   { Width     = new GridLength(1, GridUnitType.Star)  });

            MultimeterGrid.Children.Add(Screen, 0, 0);
            MultimeterGrid.Children.Add(Plot,   0, 1);
            MultimeterGrid.Children.Add(Menu,   0, 0);
            Grid.SetRowSpan(Menu, 2);

            Content = MultimeterGrid;
            Plot.FullscreenClicked += Plot_FullScreenClicked;

            SetView();
        }

        ActiveItem LastActive;
        private void Plot_FullScreenClicked(object sender, EventArgs e)
        {
            if (Item == ActiveItem.FullscreenPlot)
            {
                RequestRestore?.Invoke(this, e);
                Item = LastActive;
            }
            else
            {
                RequestMaximise?.Invoke(this, e);
                LastActive = Item;
                Item = ActiveItem.FullscreenPlot;
            }
            SetView();
        }

        private void ValueChanged ( object o, BLE.CharacteristicEvent v )
        {
            Debug.WriteLine("Recieved: " + v.NewValue);
            MyProcessor.Recieve(v.Bytes);
        }
        private void SendData(byte[] pData)
        {
            foreach (var serv in mDevice.Services)
                foreach(var chara in serv.Characteristics)
                    chara.Send(pData);
        }

        private void Menu_RangeChanged(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.RANGE);
            SendData(data);
        }
        private void Menu_ModeChanged(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.MODE);
            SendData(data);
        }
        private void Menu_RelClicked(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.REL);
            SendData(data);
        }
        private void Menu_HoldClicked(object sender, EventArgs e)
        {
            var data = Packet112GW.GetKeycode(Packet112GW.Keycode.HOLD);
            SendData(data);
        }


        private void PlotFullscreen()
        {
            Plot.IsVisible = true;
            Grid.SetRowSpan(Menu, 2);
        }
        private void PlotSmall()
        {
            Plot.IsVisible = true;
            Grid.SetRowSpan(Menu, 1);
        }
        private void PlotHide()
        {
            Plot.IsVisible = false;
        }

        private void SetView()
        {
            switch (Item)
            {
            case ActiveItem.Menu:
                Menu.IsVisible      =   true;
                Screen.IsVisible    =   false;
                PlotHide();
                break;
            case ActiveItem.Screen:
                Screen.IsVisible    =   true;
                Menu.IsVisible      =   false;
                PlotSmall();
                break;
            case ActiveItem.FullscreenPlot:
                Screen.IsVisible    =   false;
                Menu.IsVisible      =   false;
                PlotFullscreen();
                break;
            }
        }
        public void Menu_BackClicked(object sender, EventArgs e)
        {
            switch (Item)
            {
                case ActiveItem.Menu:
                    Item = ActiveItem.Screen;
                    break;
                case ActiveItem.Screen:
                    Item = ActiveItem.Menu;
                    break;
                case ActiveItem.FullscreenPlot:
                    Item = ActiveItem.Screen;
                    break;
            }
            SetView();
        }
    }
}