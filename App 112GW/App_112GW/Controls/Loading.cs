using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using App_112GW;
using System.Threading;

namespace rMultiplatform
{
    class Loading : ContentView
    {
        private TimeSpan    Now     = new TimeSpan(0, 0, 0, 0, 0);
        private TimeSpan    Period  = new TimeSpan(0, 0, 0, 0, 100);
        private Timer       Updater;

        const string dots_string = ".................";
        private int dots = 0;

        private Label LoadingText = new Label()
        {
            TextColor = Globals.TextColor,
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
        };
        private void Update()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LoadingText.Text = dots_string.Substring(dots_string.Length - dots);
                dots++; if (dots >= dots_string.Length) dots = 0;
            });
        }

        public bool IsRunning
        {
            set
            {
                if (value)  Updater = new Timer((obj) => { Update(); }, null, Now, Period);
                else        Updater = null;
            }
        }

        public Loading()
        {
            BackgroundColor = Globals.BackgroundColor;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            Updater = new Timer((obj) => { Update(); }, null, Now, Period);
            Content = LoadingText;
        }
    }
}
