using System;
using System.Threading;
using App_112GW;
using Xamarin.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
    public partial class Multimeter : AutoGrid
    {
        enum ActiveItem
        {
            Screen,
            FullscreenPlot
        }
        public BLE.IDeviceBLE       mDevice;
        public SmartChart           Chart;
        public SmartChartMenu       ChartMenu;
        public SmartChartLogger     Logger = new SmartChartLogger(10, SmartChartLogger.LoggerMode.Rescaling);
        public event EventHandler   RequestMaximise;
        public event EventHandler   RequestRestore;
        private ActiveItem          Item = ActiveItem.Screen;
        private PacketProcessor     MyProcessor = new PacketProcessor(0xF2, 26);
        private ActiveItem          LastActive;
        public MultimeterScreen     Screen;
        public MultimeterMenu       Menu;
        private string _VerticalLabel = "";

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
            }
        }

        public new string   Id      => mDevice.Id;
        public string       ShortId => Id.Substring(Id.Length - 5);
        public void         Reset() => Logger.Reset();

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

            mDevice.Change += (o, e) => { MyProcessor.Recieve(e.Bytes); };
            MyProcessor.mCallback += ProcessPacket;

            Screen  = new MultimeterScreen();
            Menu    = new MultimeterMenu(ShortId);

            #region MULTIMETER_BUTTON_EVENTS
            void SendKeycode(Packet121GW.Keycode keycode)
            {
                SendData(Packet121GW.GetKeycode(keycode));
            }
            Menu.HoldClicked        += (s, e) => { SendKeycode(Packet121GW.Keycode.HOLD);   };
            Menu.RelClicked         += (s, e) => { SendKeycode(Packet121GW.Keycode.REL);    };
            Menu.ModeChanged        += (s, e) => { SendKeycode(Packet121GW.Keycode.MODE);   };
            Menu.RangeChanged       += (s, e) => { SendKeycode(Packet121GW.Keycode.RANGE);  };
            #endregion

            #region CHART_CONSTRUCTION
            Chart = 
                new SmartChart(
                new SmartData(
                    new SmartAxisPair(
                        new SmartAxisHorizontal("Horizontal", -0.1f, 0.1f), 
                        new SmartAxisVertical("Vertical", -0.2f, 0.1f)), Logger.Data));
            Chart.Clicked += Plot_FullScreenClicked;
            #endregion

            ChartMenu = new SmartChartMenu(true, true);
            ChartMenu.SaveClicked   += (s, e) => { Chart.SaveCSV(); };
            ChartMenu.ResetClicked  += (s, e) => { Reset();         };

            DefineGrid(1, 4);

            AutoAdd(Screen);    FormatCurrentRow(GridUnitType.Star);
            AutoAdd(Menu);      FormatCurrentRow(GridUnitType.Auto);
            AutoAdd(Chart);     FormatCurrentRow(GridUnitType.Star);
            AutoAdd(ChartMenu); FormatCurrentRow(GridUnitType.Auto);

            SetView();
        }
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

        private void SendData   (byte[] pData)
        {
            foreach (var serv in mDevice.Services)
                foreach(var chara in serv.Characteristics)
                    chara.Send(pData);
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