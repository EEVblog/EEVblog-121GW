using System;
using Xamarin.Forms;

namespace rMultiplatform
{
	public class GeneralButton : Button
	{
		public GeneralButton(string pText, EventHandler pEvent)
		{
			Text = pText;
			Clicked += pEvent;
			Margin = 0;
		}
	}
}
