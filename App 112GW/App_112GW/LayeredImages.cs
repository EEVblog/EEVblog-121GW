using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
    public class ImageLayers
    {
        private bool mChange;
        public event EventHandler OnChanged;
        protected virtual void LayerChange(object o, EventArgs e)
        {
            mChange = true;
            OnChanged?.Invoke(o, e);
        }
     
        public  List<ImageLayer>	mLayers;
		public  float			    Width, Height;
		private bool			    mActive;
		private string			    mName;
        //+SKBitmap                    mLayer;
        //SKCanvas                    mCanvas;

        public ImageLayers(string pName = "")
		{
			mLayers = new List<ImageLayer>();
			mName = pName;
			mActive = false;
            mChange = true;

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
            {
                if (mChange)
                {
                    foreach (ImageLayer Layer in mLayers)
                        Layer.Render(ref pSurface);
                }
                //pSurface.DrawBitmap(mLayer, 0, 0);
                mChange = false;
            }
        }
		public void AddLayer(ImageLayer pInput)
		{
            pInput.OnChanged += LayerChange;
            mLayers.Add(pInput);

            if (pInput.mImage.Width > Width)
                Width = pInput.mImage.Width;

            if (pInput.mImage.Height > Height)
                Height = pInput.mImage.Height;

            if (mLayers.Count == 1)
            {
                mActive = true;

                //Cached resultant layer
                //(double x, double y) = GetResultSize();
                //mLayer = new SKBitmap((int)x, (int)y, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                //mCanvas = new SKCanvas(mLayer);
            }
        }
		public void AddLayer(SKImage pImage, string pName, bool pActive = true)
		{
            var temp = new ImageLayer(pImage, pName, pActive);
            AddLayer(temp);
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
