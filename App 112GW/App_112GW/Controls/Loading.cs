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
    class Loading : GeneralView
    {
        private TimeSpan    Now     = new TimeSpan(0, 0, 0, 0, 0);
        private TimeSpan    Period  = new TimeSpan(0, 0, 0, 0, 100);
        private Timer       Updater;

        const string dots_string = ".................";
        private int dots = 0;

        private GeneralLabel LoadingText = new GeneralLabel();
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
            Updater = new Timer((obj) => { Update(); }, null, Now, Period);
            Content = LoadingText;
        }
    }
}
