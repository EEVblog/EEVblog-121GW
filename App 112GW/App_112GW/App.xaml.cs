using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
namespace App_112GW
{
    [Xamarin.Forms.ContentProperty("Platforms")]
    public class Globals
    {
        static Random random = new Random();
        static public float RandBetween(float min, float max)
        {
            var output = (float)random.NextDouble() * (max - min) + min;
            return output;
        }
        static private T GetResource<T>(string name)
        {
            return (T)Application.Current.Resources[name];
        }

        static public double _BorderWidth;
        static public int BorderWidth
        {
            get
            {
                return 3;
            }
        }

        static public Color _HighlightColor;
        static public Color HighlightColor
        {
            get
            {
                switch(Device.RuntimePlatform)
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
                        return Color.FromHex("#C5CCB9");
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
