using System;
using System.Threading;
using App_112GW;
using Xamarin.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
    public partial class Multimeter : AutoGrid
    {
        public SmartChart Chart;
        public SmartChartMenu ChartMenu;
        public SmartChartLogger Logger = new SmartChartLogger(10, SmartChartLogger.LoggerMode.Rescaling);

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
                Logger.Sample(processor.MainValue);

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

        enum ActiveItem
        {
            Screen,
            FullscreenPlot
        }
        ActiveItem Item = ActiveItem.Screen;

        public void Reset()
        {
            Logger.Reset();
        }

        private string _VerticalLabel = "";
        public string VerticalLabel
        {
            set
            {
                if (value != _VerticalLabel)
                {
                    _VerticalLabel = value;
                    Chart.Title = value;
                    Reset();
                }
            }
        }

        public Multimeter ( BLE.IDeviceBLE pDevice )
        {
            mDevice = pDevice ?? throw new Exception("Multimeter must connect to a BLE device, not null.");

            mDevice.Change += ValueChanged;
            MyProcessor.mCallback += ProcessPacket;

            Screen  = new MultimeterScreen();
            Menu    = new MultimeterMenu(ShortId);

            Menu.HoldClicked        +=  Menu_HoldClicked;
            Menu.RelClicked         +=  Menu_RelClicked;
            Menu.ModeChanged        +=  Menu_ModeChanged;
            Menu.RangeChanged       +=  Menu_RangeChanged;

            Chart = 
                new SmartChart(
                new SmartData(
                    new SmartAxisPair(
                        new SmartAxisHorizontal("Horizontal", -0.1f, 0.1f), 
                        new SmartAxisVertical("Vertical", -0.2f, 0.1f)), Logger.Data));
            Chart.Clicked += Plot_FullScreenClicked;

            ChartMenu = new SmartChartMenu(true, true);
            ChartMenu.SaveClicked += Menu_SaveClicked;
            ChartMenu.ResetClicked += Menu_ResetClicked;

            DefineGrid(1, 4);
            AutoAdd(Screen);    FormatCurrentRow(GridUnitType.Star);
            AutoAdd(Menu);      FormatCurrentRow(GridUnitType.Auto);
            AutoAdd(Chart);     FormatCurrentRow(GridUnitType.Star);
            AutoAdd(ChartMenu); FormatCurrentRow(GridUnitType.Auto);

            SetView();
        }
        private void Menu_SaveClicked(object sender, EventArgs e)
        {
            Chart.SaveCSV();
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