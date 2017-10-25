using System;
using System.Collections.Generic;
using System.Text;
using Xamarin;
using Xamarin.Forms;

namespace rMultiplatform
{
	public class GeneralPage : ContentPage
	{
		public GeneralPage(string pTitle, View pContent)
		{
			Title		   = pTitle;
			Content		 = pContent;
			Padding		 = Globals.Padding;
			BackgroundColor = Globals.BackgroundColor;
		}
	}
}
