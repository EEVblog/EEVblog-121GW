using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin;

namespace rMultiplatform
{
	public class GeneralListView : ListView
	{
		public GeneralListView()
		{
			BackgroundColor	 = Globals.BackgroundColor;
			HorizontalOptions   = LayoutOptions.Fill;
			VerticalOptions	 = LayoutOptions.Fill;
		}
	}
}
