using System;
using Xamarin.Forms;
using App_112GW;

namespace rMultiplatform
{
    public class MultimeterMenu : AutoGrid
    {
        public event    EventHandler        HoldClicked;
        public event    EventHandler        RelClicked;
        public event    EventHandler        ModeChanged;
        public event    EventHandler        RangeChanged;


        private Button mMode;
        private Button mHold;
        private Button mRange;
        private Button mRelative;
        
        //
        private void    AddView    (View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            pInput.Margin = 0;
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);
            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }
        public MultimeterMenu   (string pSerialNumber = "SN0000")
        {
            Padding = 0;

            //##################################################
            HorizontalOptions   = LayoutOptions.Fill;
            VerticalOptions     = LayoutOptions.StartAndExpand;

            //##################################################
            //Add Hold Button
            mHold = new Button()
            {
                Text = "Hold", Margin = 0
            };
            mHold.Clicked += ButtonPress_Hold;
            //##################################################
            //Add Relative Button
            mRelative = new Button()
            {
                Text = "Relative",
                Margin = 0
            };
            mRelative.Clicked += ButtonPress_Relative;
            //##################################################
            //Add range dropdown
            mRange = new Button()
            {
                Text = "Range",
                Margin = 0
            };
            mRange.Clicked += PickerChange_Range;
            //##################################################
            //Add Mode dropdown
            mMode = new Button()
            {
                Text = "Mode",
                Margin = 0
            };
            mMode.Clicked += PickerChange_Mode;
            //##################################################

            DefineGrid(4, 1);
            AutoAdd(mHold);
            AutoAdd(mMode);
            AutoAdd(mRelative);
            AutoAdd(mRange);
            BackgroundColor = Globals.BackgroundColor;
        }

        private void    ButtonPress_Hold       (object sender, EventArgs e)
        {
            HoldClicked?.Invoke(sender, e);
        }
        private void    ButtonPress_Relative   (object sender, EventArgs e)
        {
            RelClicked?.Invoke(sender, e);
        }
        private void    PickerChange_Range     (object sender, EventArgs e)
        {
            RangeChanged?.Invoke(sender, e);
        }
        private void    PickerChange_Mode      (object sender, EventArgs e)
        {
            ModeChanged?.Invoke(sender, e);
        }
    }
}
