using System;
using SkiaSharp;

namespace rMultiplatform
{
	class Checkbox : GeneralControl
	{
		private bool _Checked;
		public bool Checked
		{
			get
			{
				return _Checked;
			}
		}

		public event EventHandler Changed
		{
			add
			{
				Clicked += value;
			}
			remove
			{
				Clicked -= value;
			}
		}
		protected virtual void CheckboxClick(object o, EventArgs e)
		{
			_Checked = !_Checked;
			if (_Checked)
				ShowPoints();
			else
				HidePoints();
		}
		public Checkbox() : base(new SKPoint[3]{
				new SKPoint((float)(0), (float)(0)),
				new SKPoint((float)(20), (float)(0)),
				new SKPoint((float)20, (float)(10))})
		{
			Clicked += CheckboxClick;
			OffsetAngle = 135;
		}
	}
}