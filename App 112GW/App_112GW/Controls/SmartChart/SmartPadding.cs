using System;
using SkiaSharp;

namespace rMultiplatform
{
	public class SmartPadding
	{
		//System borders
		public float Left   { set; get; }
		public float Right  { set; get; }
		public float Top	{ set; get; }
		public float Bottom { set; get; }

		//Returns the (1-Val)
		private float OtherSide(float Val)
		{
			return 1 - Val;
		}

		//These get the pixel coordinates of the padding
		public float LeftPosition(float Width)
		{
			return Left * Width;
		}
		public float RightPosition(float Width)
		{
			return OtherSide(Right) * Width;
		}
		public float TopPosition(float Height)
		{
			return Top * Height;
		}
		public float BottomPosition(float Height)
		{
			return OtherSide(Bottom) * Height;
		}
		public float PaddedWidth(float Width)
		{
			return Width * (1 - (Left + Right));
		}
		public float PaddedHeight(float Height)
		{
			return Height * (1 - (Top + Bottom));
		}
		
		//This returns the paddnig rectangle
		public SKRect Rectangle(float Height, float Width)
		{
			return new SKRect(LeftPosition(Width), TopPosition(Height), RightPosition(Width), BottomPosition(Height));
		}

		//Creates horozontal and vertical lines (pairs of points)
		public (float x1, float y1, float x2, float y2) GetHorizontalLine(float Width, float Position)
		{
			return (
				LeftPosition(Width), 
				Position, 
				RightPosition(Width), 
				Position);
		}
		public (float x1, float y1, float x2, float y2) GetVerticalLine(float Height, float Position)
		{
			return (
				Position, 
				TopPosition(Height), 
				Position, 
				BottomPosition(Height));
		}

		//To simplify code
		public SmartPadding(float L, float R, float T, float B)
		{
			if (L < 0 || R < 0 || T < 0 || B < 0)
				throw (new Exception("Padding cannot be negative"));

			Left   = L;
			Right  = R;
			Top	= T;
			Bottom = B;
		}
		public SmartPadding(float Value) : this(Value, Value, Value, Value) { }
	}
}
