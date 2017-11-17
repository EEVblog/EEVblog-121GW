using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;



namespace rMultiplatform
{
	public class SVGLayer : ILayer
	{
		public SKColor BackgroundColor
		{
			get
			{
				return mUndrawPaint.Color;
			}
			set
			{
				mUndrawPaint.Color = value;
			}
		}
		public SKColor DrawColor
		{
			get
			{
				return mDrawPaint.Color;
			}
			set
			{
				mDrawPaint.Color = value;
			}
		}

		private bool mActive;
		private VariableMonitor<bool> _Changed;
		public event EventHandler OnChanged
		{
			add
			{
				_Changed.OnChanged += value;
			}
			remove
			{
				_Changed.OnChanged -= value;
			}
		}

		public SKSvg mImage;
		public string mName;

		SKPaint mDrawPaint;
		SKPaint mUndrawPaint;

		public SVGLayer(SKSvg pImage, string pName, bool pActive = true)
		{
			_Changed = new VariableMonitor<bool>();
			_RenderChanged = new VariableMonitor<bool>();

			//Open the defined image

			mActive = pActive;
			mImage = pImage;
			mName = pName;

			//
			var transparency = Color.FromRgba(0, 0, 0, 0).ToSKColor();
			
			mDrawPaint = new SKPaint();
			mDrawPaint.Color = SKColors.Red;
			mDrawPaint.IsAntialias = true;
			mDrawPaint.BlendMode = SKBlendMode.SrcOver;
			mDrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

			mUndrawPaint = new SKPaint();
			mUndrawPaint.BlendMode = SKBlendMode.DstOut;
			mUndrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

			Off();
		}

		public void Set(bool pState)
		{
			bool temp = mActive;
			mActive = pState;
			_Changed.Update(ref mActive);
		}
		public void On()
		{
			Set(true);
		}
		public void Off()
		{
			Set(false);
		}
		public void Redraw()
		{
			_RenderChanged.UpdateOverride = true;
		}

		public override string ToString()
		{
			return mName;
		}

		private VariableMonitor<bool> _RenderChanged;

		public string   Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}
		public int	  Width
		{
			get
			{
				return (int)mImage.CanvasSize.Width;
			}
		}
		public int	  Height
		{
			get
			{
				return (int)mImage.CanvasSize.Height;
			}
		}
		public void	 Render(ref SKCanvas pSurface, SKRect pDestination)
		{
			//This is render changed variable, don't move it to set, that is wrong
			if (_RenderChanged.Update(ref mActive))
			{
				var isize = mImage.CanvasSize;
				var xscale = pDestination.Width / isize.Width;
				var yscale = pDestination.Height / isize.Height;
				var transform = SKMatrix.MakeIdentity();
				transform.SetScaleTranslate(xscale, yscale, pDestination.Left, pDestination.Top);

				if (mActive)
					pSurface.DrawPicture(mImage.Picture, ref transform, mDrawPaint);
				else
					pSurface.DrawPicture(mImage.Picture, ref transform, mUndrawPaint);
			}
		}
	}
}