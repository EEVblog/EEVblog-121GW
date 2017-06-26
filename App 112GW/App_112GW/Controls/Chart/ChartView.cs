using System;
using Xamarin.Forms;

namespace rMultiplatform
{
    public partial class ChartView : ContentView
    {
        bool                Item = true;
        public StackLayout  mChartGrid;
        private Chart       mChart;
        public ChartMenu    mMenu;

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
            mChartGrid = new StackLayout();

            // 
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.StartAndExpand;

            // Assures that a non-zero height is allocated
            MinimumHeightRequest = 200;

            //
            mMenu = new ChartMenu();
            mChart = new Chart();

            //Setup the events
            mMenu.BackClicked += ViewToggle;
            mChart.Clicked += ViewToggle;
            mMenu.SaveClicked += DataSave;


            //Add 
            mChartGrid.Children.Add(mMenu);
            mChartGrid.Children.Add(mChart);

            //
            Content = mChartGrid;
            Item = true;
            SetView();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            HeightRequest = 200;
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
    }
}