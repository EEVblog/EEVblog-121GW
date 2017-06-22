using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace App_112GW
{
    class Multimeter : Frame
    {
        public          MultimeterScreen    Screen;
        public          MultimeterMenu      Menu;
        bool                                mItem = true;

        private void SetView()
        {
            switch (mItem)
            {
                case true:
                    Content = new MultimeterScreen();
                    (Content as MultimeterScreen).Clicked += Clicked;
                    break;
                case false:
                    Content = new MultimeterMenu();
                    (Content as MultimeterMenu).Clicked += Clicked;
                    break;
                default:
                    break;
            }

            mItem = !mItem;
        }
        public          Multimeter (string pSerialNumber = "SN0000")
        {
            SetView ();
        }

        public void     Clicked(object sender, EventArgs e)
        {
            SetView();
        }
    }
}
