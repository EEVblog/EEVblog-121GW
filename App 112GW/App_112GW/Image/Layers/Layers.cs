using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace rMultiplatform
{
	public class Layers
	{
		public SKColor BackgroundColor
		{
			set
			{
				foreach (ILayer Layer in mLayers)
					Layer.BackgroundColor = value;
			}
		}
		public SKColor DrawColor
		{
			set
			{
				foreach (ILayer Layer in mLayers)
					Layer.DrawColor = value;
			}
		}

		private bool			mChange;
		public event EventHandler OnChanged;
		protected virtual void  LayerChange(object o, EventArgs e)
		{
			mChange = true;
			OnChanged?.Invoke(o, e);
		}
	 
		public  List<ILayer>	mLayers;

		public float			Width;
		public float			Height;

		private bool			mActive;
		private string			mName;

		public				  Layers(string pName = "")
		{
			mLayers	 = new List<ILayer>();
			mName	   = pName;
			mActive	 = false;
			mChange	 = true;

			mLayers.Clear();
		}

		public void			 Set(bool pState)
		{
			mActive = pState;
			foreach (ILayer Layer in mLayers)
				Layer.Set(pState);
		}
		public void			 On()
		{
			Set(true);
		}
		public void			 Off()
		{
			Set(false);
		}
		public void			 Redraw()
		{
			foreach (ILayer Layer in mLayers)
				Layer.Redraw();
			mChange = true;

			LayerChange(this, EventArgs.Empty);
		}

		public override string  ToString()
		{
			string output = "{";
			foreach (ILayer Layer in mLayers)
				output = Layer.ToString() + ", ";

			// Remove last comma and space.
			output = output.Remove(output.Length - 2) + "}";
			return output;
		}
		public (int, int)	   GetResultSize()
		{
			int x = 0;
			int y = 0;

			foreach (ILayer Layer in mLayers)
			{
				if (Layer.Width > x)
					x = Layer.Width;
				if (Layer.Height > y)
					y = Layer.Height;
			}

			return (x, y);
		}

		public void			 Render(ref SKCanvas pSurface, SKRect pRectangle)
		{
			if (mActive)
			{
				if (mChange)
					foreach (ILayer Layer in mLayers)
						Layer.Render(ref pSurface, pRectangle);
				mChange = false;
			}
		}

		public void			 AddLayer(ILayer pInput)
		{
			pInput.OnChanged += LayerChange;
			mLayers.Add(pInput);

			if (pInput.Width > Width)
				Width = pInput.Width;

			if (pInput.Height > Height)
				Height = pInput.Height;

			if (mLayers.Count == 1)
				mActive = true;
		}
	   
		public void			 AddLayer(SKImage	pImage,  string pName,   bool pActive = true)
		{
			var temp = new ImageLayer(pImage, pName, pActive);
			AddLayer(temp);
		}
		public void			 AddLayer(SKSvg	  pImage,  string pName,   bool pActive = true)
		{
			var temp = new SVGLayer(pImage, pName, pActive);
			AddLayer(temp);
		}
		public void			 AddLayer(Polycurve  pImage,  string pName,   bool pActive = true)
		{
			var temp = new PathLayer(pImage, pName, pActive);
			AddLayer(temp);
		}

		public void			 Sort()
		{
			mLayers.Sort(new LayerCompare());
		}
		public bool			 Group(string pInput, out Layers pReturn)
		{
			var temp = new Layers(mName + " " + pInput);
			foreach (ILayer layer in mLayers)
				if (layer.Name.Contains(pInput))
					temp.AddLayer(layer);

			pReturn = temp;
			if (temp.mLayers.Count > 0)
				return true;
			return false;
		}
		public void			 ToBottom(string pInput)
		{
			for (int i = 0; i < mLayers.Count; i++)
				if (mLayers[i].Name.Contains(pInput))
				{
					ILayer layer = mLayers[i];
					mLayers.Remove(layer);
					mLayers.Add(layer);
				}
		}
	}
}
