using App_112GW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace rMultiplatform
{
    public class ChartMenu : AutoGrid
    {
        private         Button          mSave;
        private         Button          mReset;
        public event    EventHandler    SaveClicked;
        public event    EventHandler    ResetClicked;

        public ChartMenu(bool ShowSave = true, bool ShowReset = true)
        {
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;
            Padding = 0;

            //Add Relative Button
            mReset = new Button() { Text = "Reset" };
            mReset.Clicked += ButtonPress_Reset;

            //Add Hold Button
            mSave = new Button() { Text = "Save" };
            mSave.Clicked += ButtonPress_Save;

            //Define Grid
            DefineGrid(2, 1);
            if (ShowReset)
                AutoAdd(mReset);
            if (ShowSave)
                AutoAdd(mSave);

            //Background Color
            BackgroundColor         = Globals.BackgroundColor;
        }

        private void ButtonPress_Save (object sender, EventArgs e)
        {
            SaveClicked?.Invoke(sender, e);
        }
        private void ButtonPress_Reset(object sender, EventArgs e)
        {
            ResetClicked?.Invoke(sender, e);
        }
    }
}
