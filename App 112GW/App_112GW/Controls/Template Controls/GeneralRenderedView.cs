using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace rMultiplatform
{
	public abstract class GeneralRenderedView : GeneralView
	{
		private GeneralRenderer mRenderer = null;
		public void Disable()
		{
            mRenderer = null;
			Content = null;
		}
		public void Enable()
		{
            mRenderer = null;
            mRenderer = new GeneralRenderer(PaintSurface);
            Content = mRenderer;
            mRenderer.InvalidateSurface();
		}
		public new bool IsVisible
		{
			set
			{
				if (value != base.IsVisible)
				{
					if (value)  Enable();
					else		Disable();

					base.IsVisible = value;
				}
			}
            get
            {
                return base.IsVisible;
            }
		}
		public SKSize CanvasSize
		{
			get
			{
				if (mRenderer != null)  return mRenderer.CanvasSize;
				return new SKSize(0, 0);
			}
		}
		public void InvalidateSurface()
		{
		    mRenderer?.InvalidateSurface();
		}
		private float ConvertWidthToPixel(float value)
		{
			return (CanvasSize.Width * value / (float)Width);
		}
		private float ConvertHeightToPixel(float value)
		{
			return (CanvasSize.Height * value / (float)Height);
		}
		public abstract void PaintSurface(SKCanvas canvas, SKSize dimension, SKSize viewsize);

		public GeneralRenderedView()
		{
			Enable();
		}
	}
}
