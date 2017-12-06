using System;
using SkiaSharp;

namespace rMultiplatform
{
	class BackButton : GeneralControl
	{
		public event EventHandler Back
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
		protected override void OnSizeAllocated(double width, double height)
		{
			width = height;
			base.OnSizeAllocated(width, height);
		}
		public BackButton () :base(new SKPoint[] {
			new SKPoint(0.5f, 0),
			new SKPoint(0, 0.5f),
			new SKPoint(1, 0.5f),
			new SKPoint(0, 0.5f),
			new SKPoint(0.5f, 1)})
		{
			ShowPoints();
		}
	}
}