using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	public class LabelledCheckbox : LabelledControl
	{
		Checkbox Control;
		public event EventHandler Changed
		{
			add
			{
				Control.Changed += value;
			}
			remove
			{
				Control.Changed -= value;
			}
		}
		public bool Checked
		{
			get
			{
				return Control.Checked;
			}
		}
		public LabelledCheckbox(string Label) : base(new Checkbox(), Label)
		{
			if (ControlView.GetType() == typeof(Checkbox))
				Control = ControlView as Checkbox;
		}
	}
}
