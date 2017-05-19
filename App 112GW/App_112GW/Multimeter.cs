using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace App_112GW
{
    class Multimeter : ContentView
    {
        private enum ShowItem
        {
            eMenu,
            eScreen
        };
        public          MultimeterScreen Screen;
        public          MultimeterMenu   Menu;

        ShowItem                         mItem;
        TapGestureRecognizer             mTapRecogniser;

        private void    SetView(ShowItem pItem)
        {
            mItem = pItem;
            SetView();
        }
        private void    SetView()
        {
            switch (mItem)
            {
                case ShowItem.eScreen:
                    Content = Screen;
                    break;
                case ShowItem.eMenu:
                    Content = Menu;
                    break;
                default:
                    break;
            }
        }
        private void    ToggleView()
        {
            if (mItem == ShowItem.eScreen)
                mItem = ShowItem.eMenu;
            else if (mItem == ShowItem.eMenu)
                mItem = ShowItem.eScreen;

            SetView();
        }

        public          Multimeter(string pSerialNumber = "SN0000")
        {
            //Add the multimeter screen
            Screen  = new MultimeterScreen();
            Screen.Clicked += Clicked;

            //Add the multimeter menu
            Menu    = new MultimeterMenu();

            //Show multimeter screen by default
            SetView(ShowItem.eMenu);

            //Setup responses to gestures
            mTapRecogniser = new TapGestureRecognizer();
            mTapRecogniser.Tapped += TapCallback;
            GestureRecognizers.Add(mTapRecogniser);
        }

        public void     Clicked(object sender, EventArgs e)
        {
            ToggleView();
        }
        private void    TapCallback(object sender, EventArgs args)
        {
            Clicked(this, EventArgs.Empty);
        }
    }
}
