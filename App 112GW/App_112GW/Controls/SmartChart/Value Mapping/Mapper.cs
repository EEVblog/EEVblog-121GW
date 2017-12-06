using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	public static class Map
	{
		public class Map1D
		{
			public float Gradient;
			public float Scale => Gradient;

			public float Offset;
			public float Translation => Offset;

			public Map1D(float m, float b)
			{
				Gradient = m;
				Offset = b;
			}
			public float Calculate(float value)
			{
				return value * Gradient + Offset;
			}
		}
		public static Map1D Create1D(float MinA, float MaxA, float MinB, float MaxB)
		{
			var SpanA = MaxA - MinA;
			var SpanB = MaxB - MinB;
			var gradient = (SpanB / SpanA);
			var offset = MinB - MinA * gradient;
			return new Map1D(gradient, offset);
		}
		public static Map1D Create1D(Range A, Range B)
		{
			return Create1D(A.Minimum, A.Maximum, B.Minimum, B.Maximum);
		}
		public static Map1D Create1D(float MinA, float MaxA, Range B)
		{
			return Create1D(MinA, MaxA, B.Minimum, B.Maximum);
		}
		public static Map1D Create1D(Range A, float MinB, float MaxB)
		{
			return Create1D(A.Minimum, A.Maximum, MinB, MaxB);
		}

		public static SkiaSharp.SKMatrix CreateMatrix(Map1D Horizontal, Map1D Vertical)
		{
			var matrix = SkiaSharp.SKMatrix.MakeIdentity();
			matrix.ScaleX = Horizontal.Scale;
			matrix.TransX = Horizontal.Translation;
			matrix.ScaleY = Vertical.Scale;
			matrix.TransY = Vertical.Translation;
			return matrix;
		}
	}
}
