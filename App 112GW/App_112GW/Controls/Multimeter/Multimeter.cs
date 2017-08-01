using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Themes;
using System.Threading;
using Xamarin.Forms;
using App_112GW;
using System.Diagnostics;

namespace rMultiplatform
{
    public partial class Multimeter : ContentView
    {
        public event EventHandler RequestMaximise;
        public event EventHandler RequestRestore;

        private BLE.IDeviceBLE mDevice;
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
                Debug.WriteLine(ex.ToString());
            }
        }

        Timer stateTimer;

        public StackLayout              MultimeterGrid;
        public MultimeterScreen         Screen;
        public MultimeterMenu           Menu;
        public ChartData                Data;
        public ChartView                Plot;

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
            if (pDevice == null)
                stateTimer = new Timer(TestCallback, null, 1000, 1000);
            else
            {
                mDevice = pDevice;
                mDevice.Change += ValueChanged;
                MyProcessor.mCallback += ProcessPacket;
            }

            // 
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.StartAndExpand;

            // Assures that a non-zero height is allocated
            MinimumHeightRequest = 200;
            
            Screen                  =   new MultimeterScreen();
            Screen.BackgroundColor  =   Globals.BackgroundColor;
            Screen.Clicked          +=  Menu_BackClicked;

            string id;
            if (pDevice == null)
                Menu = new MultimeterMenu();
            else
            {
                id = pDevice.Id;
                Menu = new MultimeterMenu(id.Substring(id.Length - 5));
            }

            Menu.BackgroundColor    =   Globals.BackgroundColor;
            Menu.BackClicked        +=  Menu_BackClicked;
            Menu.PlotClicked        +=  Menu_PlotClicked;
            Menu.HoldClicked        +=  Menu_HoldClicked;
            Menu.RelClicked         +=  Menu_RelClicked;
            Menu.ModeChanged        +=  Menu_ModeChanged;
            Menu.RangeChanged       +=  Menu_RangeChanged;

            Data = new ChartData(ChartData.ChartDataMode.eRescaling, "Time (s)", "Volts (V)", 10f);
            Plot = new ChartView() { Padding = new ChartPadding(0.1) };
            Plot.AddGrid(new ChartGrid());
            Plot.AddAxis(new ChartAxis(5, 5, 0, 20) {   Label = "Time (s)",     Orientation = ChartAxis.AxisOrientation.Horizontal, LockToAxisLabel = "Volts (V)",  LockAlignment = ChartAxis.AxisLock.eEnd, ShowDataKey = false });
            Plot.AddAxis(new ChartAxis(5, 5, 0, 0)  {   Label = "Volts (V)",    Orientation = ChartAxis.AxisOrientation.Vertical,   LockToAxisLabel = "Time (s)", LockAlignment = ChartAxis.AxisLock.eStart});
            Plot.AddData(Data);
            Plot.FullscreenClicked += Plot_FullScreenClicked;

            MultimeterGrid = new StackLayout();
            MultimeterGrid.Children.Add(Screen);
            MultimeterGrid.Children.Add(Menu);
            MultimeterGrid.Children.Add(Plot);

            Content = MultimeterGrid;
            SetView();
        }

        bool Visible = true;
        public void HideVisibleState()
        {
            Visible = IsVisible;
            IsVisible = false;
        }
        public void RestoreVisibleState()
        {
            IsVisible = Visible;
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

        private void SetView()
        {
            switch (Item)
            {
                case ActiveItem.Menu:
                    Menu.IsVisible      = true;
                    Screen.IsVisible    = false;
                    Plot.IsVisible      = Menu.PlotEnabled;
                    break;
                case ActiveItem.Screen:
                    Screen.IsVisible    = true;
                    Menu.IsVisible      = false;
                    Plot.IsVisible      = Menu.PlotEnabled;
                    break;
                case ActiveItem.FullscreenPlot:
                    Plot.IsVisible      = true;
                    Screen.IsVisible    = false;
                    Menu.IsVisible      = false;
                    break;
            }
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
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
        public void Menu_PlotClicked(object sender, EventArgs e)
        {
            SetView();
        }
    }
}