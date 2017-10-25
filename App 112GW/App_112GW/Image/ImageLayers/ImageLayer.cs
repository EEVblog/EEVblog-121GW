using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace rMultiplatform
{
	public class ImageLayer : ILayer
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

		private bool					mActive;
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

		public SKImage  mImage;
		public string   mName;

		SKPaint		 mDrawPaint;
		SKPaint		 mUndrawPaint;

		public ImageLayer(SKImage pImage, string pName, bool pActive = true)
		{
			_Changed = new VariableMonitor<bool>();
			_RenderChanged = new VariableMonitor<bool>();

			//Open the defined image
			mActive = pActive;
			mImage = pImage;
			mName = pName;

			//
			var transparency			= Color.FromRgba(0, 0, 0, 0).ToSKColor();

			mDrawPaint				  = new SKPaint();
			mDrawPaint.BlendMode		= SKBlendMode.SrcOver;
			mDrawPaint.ColorFilter	  = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

			mUndrawPaint				= new SKPaint();
			mUndrawPaint.BlendMode	  = SKBlendMode.DstOut;
			mUndrawPaint.ColorFilter	= SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

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
			{ return mName; }
			set
			{ mName = value; }
		}
		public int	  Width
		{
			get
			{
				return mImage.Width;
			}
		}
		public int	  Height
		{
			get
			{
				return mImage.Height;
			}
		}

		public void Render(ref SKCanvas pSurface, SKRect pDestination)
		{
			//This is render changed variable, don't move it to set, that is wrong
			if (_RenderChanged.Update(ref mActive))
			{
				if (mActive)
					pSurface.DrawImage(mImage, pDestination, mDrawPaint);
				else
					pSurface.DrawImage(mImage, pDestination, mUndrawPaint);
			}
		}
	}
}