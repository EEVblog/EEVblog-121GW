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
        MultimeterScreen Screen;
        MultimeterMenu Menu;
        bool Item = true;

        public MultimeterThemed (Color BackColor)
		{
			InitializeComponent ();

            Screen = new MultimeterScreen() { };
            Screen.BackgroundColor = BackColor;
            Screen.Clicked += Clicked;

            Menu = new MultimeterMenu();
            Menu.BackgroundColor = BackColor;
            Menu.Clicked += Clicked;

            SetView();
        }

        private void SetView()
        {
            switch (Item)
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
            Item = !Item;
        }

        public void Clicked(object sender, EventArgs e)
        {
            SetView();
        }
    }
}