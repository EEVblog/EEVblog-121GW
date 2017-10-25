using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace rMultiplatform
{
	class GeneralTabbedPage : Xamarin.Forms.TabbedPage
	{
		GeneralTabbedPage()
		{
			BackgroundColor = Globals.BackgroundColor;
			Padding = Globals.Padding;
			BarTextColor = Globals.TextColor;
		}
	}
}
