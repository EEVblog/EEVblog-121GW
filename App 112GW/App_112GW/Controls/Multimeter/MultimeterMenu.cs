using System;
using Xamarin.Forms;
using App_112GW;

namespace rMultiplatform
{
	public class MultimeterMenu : AutoGrid
	{
		public event	EventHandler		HoldClicked;
		public event	EventHandler		RelClicked;
		public event	EventHandler		ModeChanged;
		public event	EventHandler		RangeChanged;

		private		 GeneralButton	   mMode;
		private		 GeneralButton	   mHold;
		private		 GeneralButton	   mRange;
		private		 GeneralButton	   mRelative;

		private void	ButtonPress_Hold	   (object sender, EventArgs e)
		{
			HoldClicked?.Invoke(sender, e);
		}
		private void	ButtonPress_Relative   (object sender, EventArgs e)
		{
			RelClicked?.Invoke(sender, e);
		}
		private void	PickerChange_Range	 (object sender, EventArgs e)
		{
			RangeChanged?.Invoke(sender, e);
		}
		private void	PickerChange_Mode	  (object sender, EventArgs e)
		{
			ModeChanged?.Invoke(sender, e);
		}

		public MultimeterMenu()
		{
			//##################################################
			mHold = new GeneralButton("Hold", ButtonPress_Hold);
			mRelative = new GeneralButton("Relative", ButtonPress_Relative);
			mRange = new GeneralButton("Range", PickerChange_Range);
			mMode = new GeneralButton("Mode", PickerChange_Mode);
			//##################################################

			DefineGrid(4, 1);
			AutoAdd(mHold);
			AutoAdd(mMode);
			AutoAdd(mRelative);
			AutoAdd(mRange);
		}
	}
}
