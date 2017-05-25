using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
	public class ImageLayer
	{

        private bool                    mActive;
        private VariableMonitor<bool>   _Changed;
        public event EventHandler       OnChanged
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

		SKPaint         mDrawPaint;
        SKPaint         mUndrawPaint;

        public ImageLayer(SKImage pImage, string pName, bool pActive = true)
        {
            _Changed = new VariableMonitor<bool>();
            _RenderChanged = new VariableMonitor<bool>();

            //Open the defined image
            mActive = pActive;
            mImage = pImage;
            mName = pName;

            //
            var transparency            = Color.FromRgba(0, 0, 0, 0).ToSKColor();

            mDrawPaint                  = new SKPaint();
            mDrawPaint.BlendMode        = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter      = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            mUndrawPaint                = new SKPaint();
            mUndrawPaint.BlendMode      = SKBlendMode.DstOut;
            mUndrawPaint.ColorFilter    = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);
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
		public override string ToString()
		{
			return mName;
		}

        private VariableMonitor<bool> _RenderChanged;
        public void Render(ref SKCanvas pSurface)
        {
            //This is render changed variable, don't move it to set, that is wrong
            if (_RenderChanged.Update(ref mActive))
            {
                if (mActive)
                    pSurface.DrawImage(mImage, 0, 0, mDrawPaint);
                else
                    pSurface.DrawImage(mImage, 0, 0, mUndrawPaint);
            }
		}
	}

	public class ImageCompare : Comparer<ImageLayer>
	{
		// Compares by Length, Height, and Width.
		public override int Compare(ImageLayer x, ImageLayer y)
		{
			return x.mName.CompareTo(y.mName);
		}
	}
}
