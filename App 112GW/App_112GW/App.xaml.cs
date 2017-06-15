using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace App_112GW
{
    public class Globals
    {
        static Random random = new Random();
        static public float RandBetween(float min, float max)
        {
            var output = (float)random.NextDouble() * (max - min) + min;
            return output;
        }

        static private Object GetResource(string name)
        {
            return Application.Current.Resources[name];
        }

        static public double _BorderWidth;
        static public int BorderWidth
        {
            get
            {
                _BorderWidth = (double)GetResource("BorderWidth");
                return (int)_BorderWidth;
            }
        }

        static public Color _HighlightColor;
        static public Color HighlightColor
        {
            get
            {
                _HighlightColor = (Color)GetResource("HighlightColor");
                return _HighlightColor;
            }
        }
        static public Color _TextColor;
        static public Color TextColor
        {
            get
            {
                _TextColor = (Color)GetResource("TextColor");
                return _TextColor;
            }
        }
        static public Color _FocusColor;
        static public Color FocusColor
        {
            get
            {
                _FocusColor = (Color)GetResource("FocusColor");
                return _FocusColor;
            }
        }
        static public Color _BorderColor;
        static public Color BorderColor
        {
            get
            {
                _BorderColor = (Color)GetResource("BorderColor");
                return _BorderColor;
            }
        }
        static public Color _BackgroundColor;
        static public Color BackgroundColor
        {
            get
            {
                _BackgroundColor = (Color)GetResource("BackgroundColor");
                return _BackgroundColor;
            }
        }

        static double lasthue = 0.01;
        static private Color LastColor = Color.DodgerBlue;
        static public Color UniqueColor
        {
            get
            {
                if (lasthue > 1)
                    lasthue = lasthue - 1;
                lasthue += 0.3;

                LastColor = LastColor.WithHue(lasthue);
                return LastColor;
            }
        }
    };
	public partial class App : Application
	{
        public App ()
		{

			InitializeComponent();

			MainPage = new App_112GW.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
