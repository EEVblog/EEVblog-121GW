using System;
using Xamarin.Forms;

namespace rMultiplatform
{
    public partial class ChartView : GeneralView
    {
        private Chart       mChart;

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
            //Add
            Content = (mChart = new Chart());
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            mChart.InvalidateSurface();
            base.OnSizeAllocated(width, height);
        }
        public void DataSave(object sender, EventArgs e)
        {
            mChart.SaveCSV();
        }
    }
}