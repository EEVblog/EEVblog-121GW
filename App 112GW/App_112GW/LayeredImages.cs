using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
    public class ImageLayers
    {
		public  List<ImageLayer>	mLayers;
		public  float			    Width, Height;
		private bool			    mActive;
		private string			    mName;

		public ImageLayers(string pName = "")
		{
			mLayers = new List<ImageLayer>();
			mName = pName;
			mActive = false;
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
        public (int, int) GetResultSize()
        {
            int x = 0;
            int y = 0;

            foreach (ImageLayer Layer in mLayers)
            {
                if (Layer.mImage.Width > x)
                    x = Layer.mImage.Width;
                if (Layer.mImage.Height > y)
                    y = Layer.mImage.Height;
            }
            return (x, y);
        }
        public void Render(ref SKCanvas pSurface)
		{
            if (mActive)
                foreach (ImageLayer Layer in mLayers)
                    Layer.Render(ref pSurface);
        }
		public void AddLayer(ImageLayer pInput)
		{
            if (mLayers.Count == 0)
                mActive = true;

            mLayers.Add(pInput);

			if (pInput.mImage.Width > Width)
				Width = pInput.mImage.Width;

			if (pInput.mImage.Height > Height)
				Height = pInput.mImage.Height;
		}
		public void AddLayer(SKImage pImage, string pName, bool pActive = true)
		{
            if (mLayers.Count == 0)
                mActive = true;

            var temp = new ImageLayer(pImage, pName, pActive);
			mLayers.Add(temp);

			if (temp.mImage.Width > Width)
				Width = temp.mImage.Width;

			if (temp.mImage.Height > Height)
				Height = temp.mImage.Height;
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
