using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using rMultiplatform;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace App_112GW
{
    public class MultimeterMenu: Grid
    {
        public event EventHandler BackClicked;
        public event EventHandler   PlotClicked;
        static bool                 SameType(object A, Type B)
        {
            return Object.ReferenceEquals(A.GetType(), B);
        }

        public bool                 PlotEnabled
        {
            get
            {
                return mPlotCheck.Checked;
            }
        }

        private Button              mHold;
        private Button              mRelative;
        private Label               mSerialNumber;
        private Picker              mRange;
        private Picker              mMode;
        private Checkbox            mPlotCheck;
        private BackButton          mBack;

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
            Padding = 20;
            VerticalOptions = LayoutOptions.Center;

            //The grid is currently 2x5
            //Setup Grid rows
            RowDefinitions.Add(new RowDefinition { Height = new GridLength (1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            //Setup Grid columns
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });

            //Add Hold Button
            mHold = new Button()
            {
                Text = "Hold"
            };
            mHold.Clicked += ButtonPress_Hold;
            AddView(mHold, 0, 0);

            //Add Relative Button
            mRelative = new Button()
            {
                Text = "Relative"
            };
            mRelative.Clicked += ButtonPress_Relative;
            AddView(mRelative, 0, 1);

            //Add Relative Button
            mBack = new BackButton()
            {
                BorderWidth     = Globals.BorderWidth,
                BackgroundColor = Globals.BackgroundColor,
                PressColor      = Globals.HighlightColor,
                IdleColor       = Globals.TextColor
            };
            mBack.Clicked += ButtonPress_Back;
            AddView(mBack, 0, 2);

            //Add Plot checkbox
            mPlotCheck = new rMultiplatform.Checkbox("Plot")
            {
                BorderWidth = Globals.BorderWidth,
                BackgroundColor = Globals.BackgroundColor,
                PressColor = Globals.HighlightColor,
                IdleColor = Globals.TextColor
            };
            mPlotCheck.Clicked += CheckboxChange_Plot;
            AddView(mPlotCheck, 0, 3);

            //Add Serial number
            mSerialNumber = new Label()
            {
                Text = pSerialNumber
            };
            AddView(mSerialNumber, 1, 2);

            //Add range dropdown
            mRange = new Picker()
            {
                Title = "Select Range"
            };

            mRange.Items.Add("V");
            mRange.Items.Add("mV");
            mRange.Items.Add("uV");
            mRange.Items.Add("nV");
            mRange.Items.Add("Auto");
            mRange.SelectedIndexChanged += PickerChange_Range;
            AddView(mRange, 1, 0);

            //Add Mode dropdown
            mMode = new Picker()
            {
                Title = "Select Mode"
            };

            //
            mMode.Items.Add("DC");
            mMode.Items.Add("AC");
            mMode.SelectedIndexChanged += PickerChange_Mode;
            AddView(mMode, 1, 1);
        }

        //The reactions to picker, checkbox, buttons events
        private void    ButtonPress_Hold       (object sender, EventArgs e)
        {
        }
        private void    ButtonPress_Relative   (object sender, EventArgs e)
        {
        }
        private void    ButtonPress_Back       (object sender, EventArgs e)
        {
            if (BackClicked != null)
                BackClicked(sender, e);
        }
        private void    CheckboxChange_Plot    (object sender, EventArgs e)
        {
            if (PlotClicked != null)
                PlotClicked(mPlotCheck, e);
        }
        private void    PickerChange_Range     (object sender, EventArgs e)
        {
        }
        private void    PickerChange_Mode      (object sender, EventArgs e)
        {
        }
    }
}
