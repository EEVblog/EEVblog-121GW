using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using rMultiplatform;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using App_112GW;

namespace rMultiplatform
{
    public class MultimeterMenu : Grid
    {
        public event    EventHandler        BackClicked;
        public event    EventHandler        HoldClicked;
        public event    EventHandler        RelClicked;
        public event    EventHandler        ModeChanged;
        public event    EventHandler        RangeChanged;
        public event    EventHandler        SaveClicked;
        private         Button              mMode;
        private         Button              mHold;
        private         Button              mRange;
        private         Button              mRelative;
        private         Button              mSave;
        private         LabelledBackButton  mBack;

        static bool                 SameType(object A, Type B)
        {
            return Object.ReferenceEquals(A.GetType(), B);
        }
        public new bool IsVisible
        {
            set
            {
                if (value)
                {
                    mBack.ControlView.Enable();
                }
                else
                {
                    mBack.ControlView.Disable();
                }
                base.IsVisible = value;
            }
        }

        //
        private void    AddView    (View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }
        public          MultimeterMenu   (string pSerialNumber = "SN0000")
        {
            Padding = 10;

            //##################################################
            HorizontalOptions   = LayoutOptions.Fill;
            VerticalOptions     = LayoutOptions.Fill;

            //##################################################
            //The grid is currently 2x5
            //Setup Grid columns
            ColumnDefinitions.Add   (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });
            ColumnDefinitions.Add   (new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });

            //##################################################
            //Add Hold Button
            mHold = new Button()
            {
                Text = "Hold"
            };
            mHold.Clicked += ButtonPress_Hold;

            //##################################################
            //Add Relative Button
            mRelative = new Button()
            {
                Text = "Relative"
            };
            mRelative.Clicked += ButtonPress_Relative;

            //##################################################
            //Add Relative Button
            mSave = new Button()
            {
                Text = "Save"
            };
            mSave.Clicked += ButtonPress_Save;

            //##################################################
            //Add Relative Button
            mBack = new LabelledBackButton("Back")
            {
                BorderWidth             = Globals.BorderWidth,
                LabelSide               = LabelledControl.eLabelSide.Right,
                HorizontalOptions       = LayoutOptions.StartAndExpand,
                Padding                 = 0
            };
            mBack.Back += ButtonPress_Back;

            //##################################################
            //Add range dropdown
            mRange = new Button()
            {
                Text = "Range",
            };
            mRange.Clicked += PickerChange_Range;

            //##################################################
            //Add Mode dropdown
            mMode = new Button()
            {
                Text = "Mode",
                HorizontalOptions = LayoutOptions.Fill
            };

            //##################################################
            //
            mMode.Clicked += PickerChange_Mode;

            //##################################################

            //
            DefineRows(4);
            AutoAddRight    (mHold      );
            AutoAddRight    (mMode      );
            AutoAddRight    (mRelative  );
            AutoAddRight    (mRange     );
            AutoAddLeft     (mSave);
            SetBottomLeft   (mBack);

            BackgroundColor = Globals.BackgroundColor;
        }

        int rows = 0;
        void DefineRows(int Count)
        {
            rows = Count;
            for (var i = 0; i < Count; ++i)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        }

        int rightcount = 0;
        void AutoAddRight(View Item)
        {
            if (rightcount < rows)
                AddView(Item, 1, rightcount++);
        }
        int leftcount = 0;
        void AutoAddLeft(View Item)
        {
            if (leftcount < rows)
                AddView(Item, 0, leftcount++);
        }
        void SetBottomLeft(View Item)
        {
            AddView(Item, 0, rows - 1);
        }

        //The reactions to picker, checkbox, buttons events
        private void    ButtonPress_Hold       (object sender, EventArgs e)
        {
            HoldClicked?.Invoke(sender, e);
        }
        private void    ButtonPress_Relative   (object sender, EventArgs e)
        {
            RelClicked?.Invoke(sender, e);
        }
        private void    ButtonPress_Back       (object sender, EventArgs e)
        {
            BackClicked?.Invoke(sender, e);
        }
        private void    ButtonPress_Save       (object sender, EventArgs e)
        {
            SaveClicked?.Invoke(sender, e);
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
