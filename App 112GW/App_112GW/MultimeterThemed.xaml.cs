using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Themes;
using Xamarin.Forms;

namespace App_112GW
{
	public partial class MultimeterThemed : ContentView
	{
		public MultimeterThemed (Color BackColor, Color ForeColor)
		{
			InitializeComponent ();

            Screen = new MultimeterScreen() { };
            Screen.BackgroundColor = BackgroundColor;
            Screen.Clicked += Clicked;

            Menu = new MultimeterMenu();
            Menu.BackgroundColor = BackgroundColor;
            Menu.Clicked += Clicked;

            SetView();
        }

        MultimeterScreen Screen;
        MultimeterMenu Menu;

        bool mItem = true;
        private void SetView()
        {
            switch (mItem)
            {
                case true:
                    Content = Screen;
                    break;
                case false:
                    Content = Menu;
                    break;
                default:
                    break;
            }
            mItem = !mItem;
        }

        public void Clicked(object sender, EventArgs e)
        {
            SetView();
        }
    }
}