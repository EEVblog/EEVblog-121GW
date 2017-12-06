using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace rMultiplatform
{
	public abstract class ASmartAxis : ASmartElement
	{
		public ASmartAxisPair Parent { get; }
		public CappedRange Range { get; set; }
		public string Label { get; set; }

		public float Distance => Range.Distance;

		public ASmartTick Ticker;
		public float Position { get; set; }

		public abstract float Dimension (SKSize dimensions);
		public abstract float AxisStart (float WidthXorHeight);
		public abstract float AxisEnd   (float WidthXorHeight);
		public float          AxisSize  (float WidthXorHeight) => (AxisEnd(WidthXorHeight) - AxisStart(WidthXorHeight));

		public float ValueStart => Range.Minimum;
		public float ValueEnd   => Range.Maximum;

		public  float MinorTicks  { get; set; } = 5;
		private float MajorTicks  { get; set; } = 4;

		private float MajorTickDistance => Distance / MajorTicks;
		private float MinorTickDistance => MajorTickDistance / MinorTicks;

		//Used to interface with touch screen
		SKSize LastDimension = new SKSize(0, 0);
		public Map.Map1D ValueFromCoordinate(float dimension)
		{
			return Map.Create1D(AxisStart(dimension), AxisEnd(dimension), ValueStart, ValueEnd);
		}
		public Map.Map1D ScaleFromCoordinate(float dimension)
		{
			return Map.Create1D(0, AxisSize(dimension), 0, Range.Distance);
		}
		public Map.Map1D CoordinateFromValue(float dimension)
		{
			return Map.Create1D(ValueStart, ValueEnd, AxisStart(dimension), AxisEnd(dimension));
		}
		public void Zoom(float Amount, float About)
		{
			if (Amount <= 0) return;

			var dimension   = Dimension(LastDimension);
			var map		    = ValueFromCoordinate(dimension);
			var about	    = map.Calculate(About);
			Range.Zoom(Amount, about);
		}
		public void Pan(float Amount)
		{
			if (Amount == 0) return;

			var dimension   = Dimension(LastDimension);
			var map		    = ScaleFromCoordinate(dimension);
			var amount	    = map.Calculate(Amount);
			Range.Pan(-amount);
		}

        //
		public void Draw(SKCanvas canvas, SKSize dimension, SKSize view)
		{
            LastDimension = dimension;
			if (MajorTickDistance == 0.0)
				return;

			var draw_value_major_end = ValueEnd + MajorTickDistance / 2;
			for (Ticker.Value =  ValueStart;
				 Ticker.Value <= draw_value_major_end; 
				 Ticker.Value += MajorTickDistance)
			{
				Ticker.TickType = ASmartTick.SmartTickType.Major;
				Ticker.Draw(canvas, dimension, view);

				Ticker.TickType = ASmartTick.SmartTickType.Minor;
				var draw_value_minor_end	= Ticker.Value + MajorTickDistance - MinorTickDistance / 2;
				var draw_value_minor_start  = Ticker.Value + MinorTickDistance;
				var value = Ticker.Value;

                float last = 0;
				if (draw_value_minor_end < draw_value_major_end)
					for (Ticker.Value = draw_value_minor_start; 
						Ticker.Value <= draw_value_minor_end; 
						Ticker.Value += MinorTickDistance)
                    {
						Ticker.Draw(canvas, dimension, view);

                        //Anti-lock, floating point issue work around
                        if (last == Ticker.Value) return;
                        last = Ticker.Value;
                    }

				Ticker.Value = value;
			}
		}

		public ASmartAxis(string pLabel, float pMinimum, float pMaximum)
		{
			Label = pLabel;
			Range = new CappedRange(pMinimum, pMaximum);
		}
	}
	public class SmartAxisHorizontal : ASmartAxis
	{
		public override float AxisStart (float Width) => Padding.LeftPosition(Width);
		public override float AxisEnd   (float Width) => Padding.RightPosition(Width);

		public override float Dimension(SKSize dimensions) => dimensions.Width;
		public SmartAxisHorizontal(string pLabel, float pMinimum, float pMaximum) : base(pLabel, pMinimum, pMaximum)
		{
			Ticker = new SmartTickHorizontal(this, ASmartTick.SmartTickType.Major);
		}
	}
	public class SmartAxisVertical : ASmartAxis
	{
		public override float AxisStart (float Height) => Padding.BottomPosition(Height); 
		public override float AxisEnd   (float Height) => Padding.TopPosition(Height);

		public override float Dimension(SKSize dimensions) => dimensions.Height;
		public SmartAxisVertical(string pLabel, float pMinimum, float pMaximum) : base(pLabel, pMinimum, pMaximum)
		{
			Ticker = new SmartTickVertical(this, ASmartTick.SmartTickType.Major);
		}
	}
}