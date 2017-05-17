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

		private bool mChanged;
		private bool mActive;
		public SKBitmap mBitmap;
		private SKStream mFile;
		public string mName;

		SKPaint paint;

		public ImageLayer(string pFilename, bool pActive = true)
		{
			mActive = pActive;
			mName = System.IO.Path.GetFileName(pFilename);

			//Open the defined image
			mFile = new SKFileStream(pFilename);
			mBitmap = new SKBitmap();
			mBitmap = SKBitmap.Decode(mFile);

			paint = new SKPaint();
			var transparency = SKColors.Black; // 127 => 50%
			paint.BlendMode = SKBlendMode.Plus;
			var cf = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstIn);
			paint.ColorFilter = cf;
		}
		private void Changed()
		{
			mChanged = true;
		}
		public void Set(bool pState)
		{
			bool temp = mActive;
			mActive = pState;

			if (temp != mActive)
				Changed();
		}
		public void On()
		{
			if (mActive == false)
				Changed();

			Set(true);
		}
		public void Off()
		{
			if (mActive == true)
				Changed();

			Set(false);
		}
		public override string ToString()
		{
			return mName;
		}
		public void Render(ref SKCanvas pSurface, ref SKRect pRectangle)
		{
			if (mActive)
				pSurface.DrawBitmap(mBitmap, pRectangle, paint);
			
			mChanged = false;
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
