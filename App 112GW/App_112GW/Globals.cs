﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace App_112GW
{
    public class Globals
    {
        static private T GetResource<T>(string name)
        {
            return (T)Application.Current.Resources[name];
        }

        static Random random = new Random();
        static public double RandomBetween(double min, double max)
        {
            var output = (double)random.NextDouble() * (max - min) + min;
            return output;
        }

        static public double _BorderWidth;
        static public int BorderWidth
        {
            get
            {
                return 3;
            }
        }

        static public double Brightness(Color A)
        {
            return A.Luminosity;
        }
        static public double Contrast(Color A, Color B)
        {
            return Math.Abs(Brightness(A) - Brightness(B));
        }

        static public Color _HighlightColor;
        static public Color HighlightColor
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return Color.FromHex("#FFFFFF");
                    case Device.Android:
                        return Color.FromHex("#FFFFFF");
                    case Device.Windows:
                        return Color.FromHex("#000000");
                    case Device.WinPhone:
                        return Color.FromHex("#000000");
                }
                return Color.Black;
            }
        }

        static public Color _TextColor;
        static public Color TextColor
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return Color.FromHex("#C5CCB9");
                    case Device.Android:
                        return Color.FromHex("#FFFFFF");
                    case Device.Windows:
                        return Color.FromHex("#000000");
                    case Device.WinPhone:
                        return Color.FromHex("#000000");
                }
                return Color.Black;
            }
        }

        static public Color _FocusColor;
        static public Color FocusColor
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return Color.FromHex("#7E827A");
                    case Device.Android:
                        return Color.FromHex("#7E827A");
                    case Device.Windows:
                        return Color.FromHex("#7E827A");
                    case Device.WinPhone:
                        return Color.FromHex("#7E827A");
                }
                return Color.Black;
            }
        }

        static public Color _BackgroundColor;
        static public Color BackgroundColor
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return Color.FromHex("#111111");
                    case Device.Android:
                        return Color.FromHex("#111111");
                    case Device.Windows:
                        return Color.FromHex("#FFFFFF");
                    case Device.WinPhone:
                        return Color.FromHex("#FFFFFF");
                }
                return Color.Black;
            }
        }

        static public Color _BorderColor;
        static public Color BorderColor
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return Color.FromHex("#292F33");
                    case Device.Android:
                        return Color.FromHex("#292F33");
                    case Device.Windows:
                        return Color.FromHex("#292F33");
                    case Device.WinPhone:
                        return Color.FromHex("#292F33");
                }
                return Color.Black;
            }
        }

        static public Color UniqueColor(rMultiplatform.Range ContrastRange)
        {
            int maxtest = 0;
            while (maxtest++ < 10000)
            {
                var r = RandomBetween(0.0, 1.0);
                var g = RandomBetween(0.0, 1.0);
                var b = RandomBetween(0.0, 1.0);
                var color = new Color(r, g, b);

                if (ContrastRange.InRange(Contrast(color, BackgroundColor)))
                    return color;
            }

            return Color.DodgerBlue;
        }
    };
}
