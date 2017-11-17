using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Themes;
using Xamarin.Forms;

namespace rMultiplatform
{
	public partial class MultimeterThemed : ContentView
    {
        public StackLayout                      MultimeterGrid;
        public MultimeterScreen                 Screen;
        public MultimeterMenu                   Menu;
        public rMultiplatform.ChartData         Data;
        public rMultiplatform.Chart             Plot;
        bool Item = true;

        public MultimeterThemed (Color BackColor)
        {
            HorizontalOptions       = LayoutOptions.Fill;
            VerticalOptions         = LayoutOptions.StartAndExpand;

            //Assures that a non-zero height is allocated
            MinimumHeightRequest    = 200;

            // InitializeComponent ();
            Screen = new MultimeterScreen();
            Screen.BackgroundColor = BackColor;
            Screen.Clicked += BackClicked;

            Menu = new MultimeterMenu();
            Menu.BackgroundColor = BackColor;
            Menu.BackClicked += BackClicked;
            Menu.PlotClicked += PlotClicked;

            Data = new rMultiplatform.ChartData(rMultiplatform.ChartData.ChartDataMode.eRolling, "Time (s)", "Volts (V)", 0.1f, 10.0f);
            Plot = new rMultiplatform.Chart() { Padding = new rMultiplatform.ChartPadding(0.1) };
            Plot.AddGrid(new rMultiplatform.ChartGrid());
            Plot.AddAxis(new rMultiplatform.ChartAxis(5, 5, 0, 20) { Label = "Time (s)", Orientation = rMultiplatform.ChartAxis.AxisOrientation.Horizontal, AxisLocation = 0.9, LockToAxisLabel = "Volts (V)", LockAlignment = rMultiplatform.ChartAxis.AxisLock.eMiddle });
            Plot.AddAxis(new rMultiplatform.ChartAxis(5, 5, 0, 0) { Label = "Volts (V)", Orientation = rMultiplatform.ChartAxis.AxisOrientation.Vertical, AxisLocation = 0.1, LockToAxisLabel = "Time (s)", LockAlignment = rMultiplatform.ChartAxis.AxisLock.eStart });
            Plot.AddData(Data);

            Menu.IsVisible      = true;
            Screen.IsVisible    = false;
            Plot.IsVisible      = false;
            
            MultimeterGrid = new StackLayout();
            MultimeterGrid.Children.Add(Screen);
            MultimeterGrid.Children.Add(Menu);
            MultimeterGrid.Children.Add(Plot);

            Content = MultimeterGrid;
            SetView();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }
        private void            SetView()
        {
            switch (Item)
            {
                case true:
                    Menu.IsVisible          = true;
                    Screen.IsVisible        = false;
                    break;
                case false:
                    Menu.IsVisible          = false;
                    Screen.IsVisible        = true;
                    break;
                default:
                    break;
            }

            Plot.IsVisible = Menu.PlotEnabled;
        }
        public void             BackClicked(object sender, EventArgs e)
        {
            Item = !Item;
            SetView();
        }
        public void             PlotClicked(object sender, EventArgs e)
        {
            SetView();
        }
    }
}