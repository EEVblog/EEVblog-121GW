using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	public abstract class ASmartTick : ASmartElement
	{
		public ASmartAxis Parent { get; private set; }
		public float Value;
		public enum SmartTickType
		{
			Major, Minor
		}
		public SmartTickType TickType;

		public float Position(float dimension) => Parent.CoordinateFromValue(dimension).Calculate(Value);

		public static bool  ShowTick			 { get; set; } = true;
		public static bool  ShowMajorLabel	   { get; set; } = true;
		public static bool  ShowMajorGridline	{ get; set; } = true;
		public static bool  ShowMinorGridline	{ get; set; } = false;

		public bool ShowGridline    => (TickType == SmartTickType.Major) ? ShowMajorGridline : ShowMinorGridline;
		public bool IsMajorTick     => (TickType == SmartTickType.Major);
		public bool IsMinorTick     => (TickType == SmartTickType.Minor);

		private static float _MajorTickLength = 5.0f;
		private static float MajorTickLength
		{
			get
			{
				return _MajorTickLength;
			}
			set
			{
				_MajorTickLength = value;
			}
		}
		private static float MinorTickLength
		{
			get
			{
				return MajorTickLength / 2;
			}
		}

		public float			TickLength
		{
			get
			{
				return (TickType == SmartTickType.Major)? MajorTickLength : MinorTickLength;
			}
		}

		public enum TickLabelSide
		{
			Inside,
			Outside
		}
		public TickLabelSide LabelSide { get; set; } = TickLabelSide.Inside;

		protected abstract (float x, float y) TickStart (SKSize dimensoin);
		protected abstract (float x, float y) TickEnd   (SKSize dimensoin);
		protected abstract (float x, float y) TickCentre(SKSize dimension);
		private (float x1, float y1, float x2, float y2) TickLine(SKSize dimension)
		{
			(var start_x, var start_y) = TickStart(dimension);
			(var end_x, var end_y) = TickEnd(dimension);
			return (start_x, start_y, end_x, end_y);
		}

		protected abstract (float x1, float y1, float x2, float y2) GridLine(SKSize dimension);

		protected abstract (SKPoint x, SKPoint y) LabelLine(float scale, SKSize dimension, string Text);
		private (string, SKPath) LabelPath(float scale, SKSize dimension)
		{
			var txt = SIPrefix.ToString(Value);
			(var pt1, var pt2) = LabelLine(scale, dimension, txt);
			var pts = new SKPoint[] { pt1, pt2 };
			var pth = new SKPath();
			pth.AddPoly(pts, false);
			return (txt, pth);
		}

		public void Draw ( SKCanvas canvas, SKSize dimension, SKSize view )
        {
            (var scalex, var scaley) = SmartDPI.GetScale(canvas, dimension, view);
            if (ShowTick)
            {
                (var start_x, var start_y, var end_x, var end_y) = TickLine(dimension);
                var temp_paint = MajorPaint(scaley);
                canvas.DrawLine(start_x, start_y, end_x, end_y, temp_paint);

				if (IsMajorTick)
				{
					if (ShowMajorLabel)
					{
                        //Handles different DPI
						(var txt, var pth) = LabelPath(scaley, dimension);
                        canvas.DrawTextOnPath(txt, pth, 0, 0, temp_paint);
					}
				}
			}
			if (ShowGridline)
			{
				(var x1, var y1, var x2, var y2) = GridLine(dimension);
				canvas.DrawLine(x1, y1, x2, y2, GridPaint(scaley));
			}
		}

		public ASmartTick(ASmartAxis pParent, SmartTickType Type)
		{
			Parent = pParent;
			TickType = Type;
		}
	}
	public class SmartTickHorizontal : ASmartTick
	{
		protected override (float x, float y)   TickStart   (SKSize dimension) => (Position(dimension.Width), Parent.Position - TickLength);
		protected override (float x, float y)   TickEnd	    (SKSize dimension) => (Position(dimension.Width), Parent.Position + TickLength);
		protected override (float x, float y)   TickCentre  (SKSize dimension) => (Position(dimension.Width), Parent.Position);

		protected override (float x1, float y1, float x2, float y2) GridLine(SKSize dimension) => Padding.GetVerticalLine(dimension.Height, Position(dimension.Width));
        protected override (SKPoint x, SKPoint y) LabelLine(float scale, SKSize dimension, string Text)
		{
            var temp_paint = MajorPaint(scale);
            var space_width = SpaceWidth(temp_paint);
			(var wid,   var hei)    = MeasureText(Text, temp_paint);
			(var tx,	var ty)	    = TickEnd(dimension);

			if (LabelSide == TickLabelSide.Inside)
			{
				ty -= space_width * 3;
				tx -= space_width;
				var pt1 = new SKPoint(tx, ty);
				var pt2 = new SKPoint(tx, ty - wid * scale);
				return (pt1, pt2);
			}
			else
			{
				var pt1 = new SKPoint(tx, ty + space_width + wid * scale);
				var pt2 = new SKPoint(tx, ty + space_width);
				return (pt1, pt2);
			}
		}

		public SmartTickHorizontal(ASmartAxis Parent, SmartTickType Type) : base(Parent, Type) { }
	}
	public class SmartTickVertical : ASmartTick
	{
		protected override (float x, float y)   TickStart   (SKSize dimension) => (Parent.Position - TickLength,	Position(dimension.Height));
		protected override (float x, float y)   TickEnd	    (SKSize dimension) => (Parent.Position + TickLength,    Position(dimension.Height));
		protected override (float x, float y)   TickCentre  (SKSize dimension) => (Parent.Position,				    Position(dimension.Height));

		protected override (float x1, float y1, float x2, float y2) GridLine(SKSize dimension) => Padding.GetHorizontalLine(dimension.Width, Position(dimension.Height));
		protected override (SKPoint x, SKPoint y) LabelLine(float scale, SKSize dimension, string Text)
		{
            var temp_paint = MajorPaint(scale);
			(var wid,   var hei) = MeasureText(Text, temp_paint);
            var space = SpaceWidth(temp_paint);

            (var tx,	var ty)	 = TickStart(dimension);
			if (LabelSide == TickLabelSide.Inside)
			{
				tx += space;

                var pt1 = new SKPoint(tx , ty);
				var pt2 = new SKPoint(tx + wid, ty);

				return (pt1, pt2);
			}
			else
			{
				var pt1 = new SKPoint(tx - space - wid * scale, ty);
				var pt2 = new SKPoint(tx - space, ty);

				return (pt1, pt2);
			}
		}

		public SmartTickVertical(ASmartAxis Parent, SmartTickType Type) : base(Parent, Type) { }
	}
}