using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace App_112GW
{
    class Multimeter : Grid
    {
        const string MultimeterLayer = "./Layers";
        TimeSpan MenuTimeout = new TimeSpan(0, 0, 3);
        
        private MultimeterScreen    mScreen;
        private Button              mHold;
        private Button              mRelative;
        private Label               mSerialNumber;
        private Picker              mRange;
        private Picker              mMode;
        private Checkbox            mPlotCheck;
        private Checkbox            mAutorangeCheck;

        private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }

        public Multimeter(string pSerialNumber = "SN0000")
        {
            //The grid is currently 2x5
            //Setup Grid rows
            for (int a = 0; a < 5; a++)
                RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            //Setup Grid columns
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //Add the multimeter screen
            mScreen = new MultimeterScreen(MultimeterLayer, MenuTimeout);
            AddView(mScreen, 0, 0, 2, 5);

            //Add the multimeter memu

            //Add Hold Button
            mHold = new Button()
            {
                Text = "Hold",
                Style = Globals.ButtonStyle
            };
            AddView(mHold, 0, 0);

            //Add Relative Button
            mRelative = new Button()
            {
                Text = "Relative",
                Style = Globals.ButtonStyle
            };
            AddView(mRelative, 0, 1);

            //Add Plot checkbox
            mPlotCheck = new Checkbox("Plot");
            AddView(mPlotCheck, 1, 0);

            //Add Autorange checkboxs
            mAutorangeCheck = new Checkbox("Autorange");
            AddView(mAutorangeCheck, 1, 1);

            //Add range dropdown
            mRange = new Picker()
            {
                Title = "Select Range"
            };
            mRange.Items.Add("V");
            mRange.Items.Add("mV");
            mRange.Items.Add("uV");
            mRange.Items.Add("nV");
            AddView(mRange, 1, 2);

            //Add Mode dropdown
            mMode = new Picker()
            {
                Title = "Select Mode"
            };
            mMode.Items.Add("DC");
            mMode.Items.Add("AC");
            AddView(mMode, 1, 3);

            //Add Serial number
            mSerialNumber = new Label()
            {
                Text = pSerialNumber,
                Style = Globals.LabelStyle
            };
            AddView(mSerialNumber, 1, 4);
        }
    }
}
