using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace rMultiplatform
{
	public class PathLayer : ILayer
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

		public string				   mName;
		public Polycurve				mImage;
		private bool					mActive;
		private SKPaint				 mDrawPaint;
		private SKPaint				 mUndrawPaint;

		private VariableMonitor<bool>   _RenderChanged;
		private VariableMonitor<bool>   _Changed;
		public event EventHandler	   OnChanged
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

		public PathLayer(Polycurve pImage, string pName, bool pActive = true)
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
			mDrawPaint.BlendMode = SKBlendMode.Src;
			mDrawPaint.Color = Globals.TextColor.ToSKColor();
			mDrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.Dst);

			mUndrawPaint = new SKPaint();
			mUndrawPaint.BlendMode = SKBlendMode.Src;
			mUndrawPaint.Color = Globals.BackgroundColor.ToSKColor();
			mUndrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.Dst);

			Off();
		}

		public void	 Set(bool pState)
		{
			bool temp = mActive;
			mActive = pState;
			_Changed.Update(ref mActive);
		}
		public void	 On()
		{
			Set(true);
		}
		public void	 Off()
		{
			Set(false);
		}
		public void	 Redraw()
		{
			_Changed.UpdateOverride = true;
			_RenderChanged.UpdateOverride = true;
		}

		public override string ToString()
		{
			return mName;
		}

		public string   Name
		{
			get
			{ return mName; }
			set
			{ mName = value; }
		}
		public int	  Width
		{
			get
			{
				return (int)mImage.Width;
			}
		}
		public int	  Height
		{
			get
			{
				return (int)mImage.Height;
			}
		}

		public void Render (ref SKCanvas pSurface, SKRect pDestination)
		{
			//This is render changed variable, don't move it to set, that is wrong
			if (_RenderChanged.Update(ref mActive))
			{
				var isize   = mImage.CanvasSize;

				var xscale  = pDestination.Width / isize.Width;
				var yscale  = pDestination.Height / isize.Height;

				var transform = SKMatrix.MakeIdentity();
				transform.SetScaleTranslate(xscale, yscale, pDestination.Left, pDestination.Top);

				//If the item is inside the another path then it should undraw it whendraw operation takes place.
				if (mActive)	mImage.Draw (ref pSurface, transform, ref mDrawPaint, ref mUndrawPaint);
				else			mImage.Draw (ref pSurface, transform, ref mUndrawPaint, ref mUndrawPaint);
			}
		}
	}
}