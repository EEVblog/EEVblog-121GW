using App_112GW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace rMultiplatform
{
    public class ChartMenu : Grid
    {
        public event EventHandler SaveClicked;
        public event EventHandler BackClicked;
        public event EventHandler FullscreenClicked;
        static bool SameType(object A, Type B)
        {
            return Object.ReferenceEquals(A.GetType(), B);
        }

        private Button              mSave;
        private LabelledBackButton  mBack;
        private Button mFullscreen;

        public new bool IsVisible
        {
            set
            {
                if (value)
                    mBack.ControlView.Enable();
                else
                    mBack.ControlView.Disable();
                base.IsVisible = value;
            }
        }

        private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }

        public ChartMenu()
        {
            //##################################################
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Center;
            Padding = 10;

            //##################################################
            //The grid is currently 2x5
            //Setup Grid rows
            RowDefinitions.Add  (   new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } );
            RowDefinitions.Add  (   new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            //##################################################
            //Setup Grid columns
            ColumnDefinitions.Add ( new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } );
            ColumnDefinitions.Add ( new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } );
           
            //##################################################
            //Add Relative Button
            mBack = new LabelledBackButton ("Back")
            {
                BorderWidth         = Globals.BorderWidth,
                LabelSide           = LabelledControl.eLabelSide.Right,
                HorizontalOptions   = LayoutOptions.StartAndExpand,
                Padding             = 0
            };
            mBack.Back += ButtonPress_Back;

            //Add Hold Button
            mSave = new Button() { Text = "Save" };
            mSave.Clicked += ButtonPress_Save;

            mFullscreen = new Button() { Text = "Fullscreen" };
            mFullscreen.Clicked += ButtonPress_Fullscreen;

            //
            AddView(mBack, 0, 0);
            AddView(mSave, 1, 0);
            AddView(mFullscreen, 1, 1);
            //##################################################
            BackgroundColor         = Globals.BackgroundColor;
            mSave.BackgroundColor   = Globals.BackgroundColor;
            mSave.TextColor         = Globals.TextColor;
        }


        //The reactions to picker, checkbox, buttons events
        private void ButtonPress_Fullscreen(object sender, EventArgs e)
        {
            Debug.WriteLine("ButtonPress_Fullscreen.");
            FullscreenClicked?.Invoke(sender, e);
        }
        private void ButtonPress_Save (object sender, EventArgs e)
        {
            SaveClicked?.Invoke(sender, e);
        }
        private void ButtonPress_Back (object sender, EventArgs e)
        {
            BackClicked?.Invoke(sender, e);
        }
    }
}
