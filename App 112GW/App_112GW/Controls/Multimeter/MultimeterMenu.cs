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
    public class MultimeterMenu: Grid
    {
        public event EventHandler   BackClicked;
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
        private LabelledCheckbox    mPlotCheck;
        private LabelledBackButton  mBack;

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
            //##################################################
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions     = LayoutOptions.StartAndExpand;
            Padding             = 10;

            //##################################################
            //The grid is currently 2x5
            //Setup Grid rows
            RowDefinitions.Add(new RowDefinition { Height = new GridLength (1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            //Setup Grid columns
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) });

            //##################################################
            //Add Hold Button
            mHold = new Button()
            {
                Text = "Hold"
            };
            mHold.Clicked += ButtonPress_Hold;
            AddView(mHold, 0, 0);

            //##################################################
            //Add Relative Button
            mRelative = new Button()
            {
                Text = "Relative"
            };
            mRelative.Clicked += ButtonPress_Relative;
            AddView(mRelative, 0, 1);

            //##################################################
            //Add Relative Button
            var Stack = new StackLayout()
            {
                Orientation             = StackOrientation.Horizontal,
                HorizontalOptions       = LayoutOptions.FillAndExpand,
                VerticalOptions         = LayoutOptions.CenterAndExpand,
            };
            mBack = new LabelledBackButton("Back")
            {
                BorderWidth             = Globals.BorderWidth,

                LabelSide               = LabelledControl.eLabelSide.Right,
                HorizontalOptions       = LayoutOptions.StartAndExpand,
                Padding                 = 0
            };
            mBack.Back += ButtonPress_Back;

            //##################################################
            //Add Plot checkbox
            mPlotCheck = new LabelledCheckbox("Plot")
            {
                BorderWidth             = Globals.BorderWidth,
                LabelSide               = LabelledControl.eLabelSide.Left,
                HorizontalOptions       = LayoutOptions.EndAndExpand,
                Padding                 = 0
            };

            //##################################################
            //Add Serial number
            mSerialNumber = new Label()
            {
                Text                    = pSerialNumber,
                VerticalOptions         = LayoutOptions.CenterAndExpand,
                HorizontalOptions       = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center
            };
            mPlotCheck.Changed += CheckboxChange_Plot;
            Stack.Children.Add(mBack);
            Stack.Children.Add(mSerialNumber);
            Stack.Children.Add(mPlotCheck);
            AddView(Stack, 0 ,2, 2, 1);

            //##################################################
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

            //##################################################
            //Add Mode dropdown
            mMode = new Picker()
            {
                Title = "Select Mode",
                HorizontalOptions = LayoutOptions.Fill
            };

            //##################################################
            //
            mMode.Items.Add("DC");
            mMode.Items.Add("AC");
            mMode.SelectedIndexChanged += PickerChange_Mode;
            AddView(mMode, 1, 1);

            //##################################################
            //
            BackgroundColor = Globals.BackgroundColor;
            mMode.BackgroundColor = Globals.BackgroundColor;
            mMode.TextColor = Globals.TextColor;
            mRange.BackgroundColor = Globals.BackgroundColor;
            mRange.TextColor = Globals.TextColor;
            mHold.BackgroundColor = Globals.BackgroundColor;
            mHold.TextColor = Globals.TextColor;
            mRelative.BackgroundColor = Globals.BackgroundColor;
            mRelative.TextColor = Globals.TextColor;
        }

        //The reactions to picker, checkbox, buttons events
        private void    ButtonPress_Hold       (object sender, EventArgs e)
        {}
        private void    ButtonPress_Relative   (object sender, EventArgs e)
        {}
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
        {}
        private void    PickerChange_Mode      (object sender, EventArgs e)
        {}
    }
}
