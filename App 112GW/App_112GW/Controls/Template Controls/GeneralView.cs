using System;
using System.Collections.Generic;
using System.Text;
using Xamarin;
using Xamarin.Forms;

namespace rMultiplatform
{
	public class GeneralView : ContentView
	{
		public GeneralView()
		{
			//Must always fill parent container
			HorizontalOptions   = LayoutOptions.Fill;
			VerticalOptions	 = LayoutOptions.Fill;
			Padding			 = Globals.Padding;
			BackgroundColor	 = Globals.BackgroundColor;
		}
	}
}
