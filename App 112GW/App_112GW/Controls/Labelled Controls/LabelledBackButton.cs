using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	public class LabelledBackButton : LabelledControl
	{
		BackButton Control;
		public event EventHandler Back
		{
			add
			{Control.Back += value;}
			remove
			{Control.Back -= value;}
		}
		public LabelledBackButton(string Label) :base(new BackButton(), Label)
		{
			if (ControlView.GetType() == typeof(BackButton))
				Control = ControlView as BackButton;
		}
	}
}
