using System;
using System.Threading;
using App_112GW;
using Xamarin.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
    public partial class Multimeter : AutoGrid
    {
        public event EventHandler RequestMaximise;
        public event EventHandler RequestRestore;

        public BLE.IDeviceBLE mDevice;
        PacketProcessor MyProcessor = new PacketProcessor(0xF2, 26);
        void ProcessPacket(byte[] pInput)
        {
            var processor = new Packet121GW();
            try
            {
                processor.ProcessPacket(pInput);
                Data.Sample(processor.MainValue);

                VerticalLabel = processor.MainRangeLabel;
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
        public string ShortId
        {
            get
            {
                var id = Id;
                return Id.Substring(id.Length - 5);
            }
        }
        public MultimeterScreen Screen;
        public MultimeterMenu   Menu;
        public ChartMenu        ChartMenu;
        public ChartData        Data;
        public Chart            _Plot;
        public Chart            Plot
        {
            get
            {
                return _Plot;
            }
            private set
            {
                _Plot = value;
            }
        }

        enum ActiveItem
        {
            Screen,
            FullscreenPlot
        }
        ActiveItem Item = ActiveItem.Screen;

        public void Reset()
        {
            Data.Data.Clear();
            Data.Reset();
            VerticalAxis.Reset();
            HorozontalAxis.Reset();
        }

        ChartAxis VerticalAxis;
        ChartAxis HorozontalAxis;
        private string _VerticalLabel = "Volts (V)";
        public string VerticalLabel
        {
            get
            {
                return _VerticalLabel;
            }
            set
            {
                if (value != _VerticalLabel)
                {
                    _VerticalLabel                  = value;
                    Data.VerticalLabel              = value;
                    VerticalAxis.Label              = value;
                    HorozontalAxis.LockToAxisLabel  = value;
                    Reset();
                }
            }
        }

        public Multimeter ( BLE.IDeviceBLE pDevice )
        {
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;

            Padding = 0;
            mDevice = pDevice ?? throw new Exception("Multimeter must connect to a BLE device, not null.");

            mDevice.Change += ValueChanged;
            MyProcessor.mCallback += ProcessPacket;

            Screen  = new MultimeterScreen();
            Menu    = new MultimeterMenu(ShortId);

            Menu.HoldClicked        +=  Menu_HoldClicked;
            Menu.RelClicked         +=  Menu_RelClicked;
            Menu.ModeChanged        +=  Menu_ModeChanged;
            Menu.RangeChanged       +=  Menu_RangeChanged;

            Data = new ChartData( ChartData.ChartDataMode.eRescaling, "Time (s)", _VerticalLabel, 10f );
            Plot = new Chart() {Padding = new ChartPadding(0.05)};
            Plot.AddGrid ( new ChartGrid() );
            Plot.AddAxis ( HorozontalAxis    = new ChartAxis(5, 5, 0, 20){   Label = "Time (s)",     Orientation = ChartAxis.AxisOrientation.Horizontal, LockToAxisLabel = _VerticalLabel,   LockAlignment = ChartAxis.AxisLock.eEnd, ShowDataKey = false } );
            Plot.AddAxis ( VerticalAxis      = new ChartAxis(5, 5, 0, 0) {   Label = _VerticalLabel, Orientation = ChartAxis.AxisOrientation.Vertical,   LockToAxisLabel = "Time (s)",       LockAlignment = ChartAxis.AxisLock.eStart} );
            Plot.AddData(Data);
            Plot.FullscreenClicked += Plot_FullScreenClicked;

            ChartMenu = new ChartMenu(true, true);
            ChartMenu.SaveClicked += Menu_SaveClicked;
            ChartMenu.ResetClicked += Menu_ResetClicked;

            DefineGrid(1, 4);
            AutoAdd(Screen);    FormatCurrentRow(GridUnitType.Auto);
            AutoAdd(Menu);      FormatCurrentRow(GridUnitType.Auto);
            AutoAdd(Plot);      FormatCurrentRow(GridUnitType.Star);
            AutoAdd(ChartMenu); FormatCurrentRow(GridUnitType.Auto);

            BackgroundColor = Globals.BackgroundColor;
            SetView();
        }

        private void Menu_SaveClicked(object sender, EventArgs e)
        {
            Plot.SaveCSV();
        }

        private void Menu_ResetClicked(object sender, EventArgs e)
        {
            Reset();
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

        private void SendKeycode(Packet121GW.Keycode keycode)
        {
            SendData(Packet121GW.GetKeycode(keycode));
        }
        private void Menu_RangeChanged(object sender, EventArgs e)
        {
            SendKeycode(Packet121GW.Keycode.RANGE);
        }
        private void Menu_ModeChanged(object sender, EventArgs e)
        {
            SendKeycode(Packet121GW.Keycode.MODE);
        }
        private void Menu_RelClicked(object sender, EventArgs e)
        {
            SendKeycode(Packet121GW.Keycode.REL);
        }
        private void Menu_HoldClicked(object sender, EventArgs e)
        {
            SendKeycode(Packet121GW.Keycode.HOLD);
        }

        private void SetView()
        {
            switch (Item)
            {
            case ActiveItem.Screen:
                Screen.IsVisible    =   true;
                Menu.IsVisible      =   true;
                break;
            case ActiveItem.FullscreenPlot:
                Screen.IsVisible    =   false;
                Menu.IsVisible      =   false;
                break;
            }
        }
    }
}