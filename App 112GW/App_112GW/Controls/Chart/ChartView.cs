using System;
using Xamarin.Forms;

namespace rMultiplatform
{
    public partial class ChartView : Grid
    {
        public event EventHandler FullscreenClicked
        {
            add
            {
                mChart.FullscreenClicked += value;
            }
            remove
            {
                mChart.FullscreenClicked -= value;
            }
        }
        
        bool                    Item = true;
        public  StackLayout     mChartGrid;
        private Chart           mChart;
        public  ChartMenu       mMenu;

        //Wrappers for the supported chart elements
        public void AddAxis(ChartAxis pInput)
        {
            mChart.AddAxis(pInput);
        }
        public void AddData(ChartData pInput)
        {
            mChart.AddData(pInput);
        }
        public void AddGrid(ChartGrid pInput)
        {
            mChart.AddGrid(pInput);
        }
        
        //Defines the padding around the boarder of the control
        public new ChartPadding  Padding
        {
            set
            {
                mChart.Padding = value;
            }
            get
            {
                return mChart.Padding;
            }
        }

        public ChartView()
        {
            //Setup the events
            mChart = new Chart();
            mMenu = new ChartMenu();
            mMenu.BackClicked   += ViewToggle;
            mMenu.SaveClicked   += DataSave;
            mChart.Clicked      += ViewToggle;
            FullscreenClicked   += FullscreenToggle;

            //Add
            Children.Add(mMenu);
            Children.Add(mChart);

            Item = true;
            SetView();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }
        private void SetView()
        {
            switch (Item)
            {
                case true:
                    mMenu.IsVisible = true;
                    mChart.IsVisible = false;
                    break;
                case false:
                    mMenu.IsVisible = false;
                    mChart.IsVisible = true;
                    break;
                default:
                    break;
            }
        }
        public void DataSave(object sender, EventArgs e)
        {
            mChart.SaveCSV();
        }
        public void ViewToggle(object sender, EventArgs e)
        {
            Item = !Item;
            SetView();
        }
        private void FullscreenToggle(object sender, EventArgs e)
        {
            SetView();
        }
    }
}