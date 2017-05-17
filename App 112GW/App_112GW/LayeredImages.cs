using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
    public class ImageLayers
    {
		public List<ImageLayer>	mLayers;
		public float			Width, Height;
		private bool			mActive;
		private string			mName;

		public ImageLayers(string pName = "", bool pActive = true)
		{
			mLayers = new List<ImageLayer>();
			mName = pName;
			mActive = pActive;
			mLayers.Clear();
		}
		public void Set(bool pState)
		{
			mActive = pState;
			foreach (ImageLayer Layer in mLayers)
				Layer.Set(pState);
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
			string output = "{";
			foreach (ImageLayer Layer in mLayers)
				output = Layer.ToString() + ", ";

			// Remove last comma and space.
			output = output.Remove(output.Length - 2) + "}";
			return output;
		}
		public void Render(ref SKCanvas pSurface, ref SKRect pRectangle)
		{
			if (mActive)
				foreach (ImageLayer Layer in mLayers)
					Layer.Render(ref pSurface, ref pRectangle);
		}
		
		public void AddLayer(ImageLayer pInput)
		{
			mLayers.Add(pInput);

			if (pInput.mBitmap.Width > Width)
				Width = pInput.mBitmap.Width;

			if (pInput.mBitmap.Height > Height)
				Height = pInput.mBitmap.Height;
		}
		public void AddLayer(string pFilename, bool pActive = true)
		{
			var temp = new ImageLayer(pFilename, pActive);

			mLayers.Add(temp);

			if (temp.mBitmap.Width > Width)
				Width = temp.mBitmap.Width;

			if (temp.mBitmap.Height > Height)
				Height = temp.mBitmap.Height;
		}
		public void Sort()
		{
			mLayers.Sort(new ImageCompare());
		}
		
		public bool Group(string pInput, out ImageLayers pReturn)
		{
			ImageLayers temp = new ImageLayers(mName + " " + pInput);
			foreach (ImageLayer layer in mLayers)
				if (layer.mName.Contains(pInput))
					temp.AddLayer(layer);

			pReturn = temp;
			if (temp.mLayers.Count > 0)
				return true;
			return false;
		}
		public void ToBottom(string pInput)
		{
			for (int i = 0; i < mLayers.Count; i++)
				if (mLayers[i].mName.Contains(pInput))
				{
					ImageLayer layer = mLayers[i];
					mLayers.Remove(layer);
					mLayers.Add(layer);
				}
		}
	}
}
