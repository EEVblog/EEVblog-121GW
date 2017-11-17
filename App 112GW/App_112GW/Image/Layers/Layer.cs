using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace rMultiplatform
{
	public interface ILayer
	{
		SKColor BackgroundColor
		{
			get;
			set;
		}
		SKColor DrawColor
		{
			get;
			set;
		}

		event EventHandler	  OnChanged;
		void					Set(bool pState);
		void					Off();
		void					On();
		void					Redraw();

		string				  ToString();
		string				  Name
		{
			get;
			set;
		}
		void					Render(ref SKCanvas pSurface, SKRect pDestination);

		int Width
		{
			get;
		}
		int Height
		{
			get;
		}
	}
	public class LayerCompare : Comparer<ILayer>
	{
		// Compares by Length, Height, and Width.
		public override int Compare(ILayer x, ILayer y)
		{
			return x.Name.CompareTo(y.Name);
		}
	}
}
