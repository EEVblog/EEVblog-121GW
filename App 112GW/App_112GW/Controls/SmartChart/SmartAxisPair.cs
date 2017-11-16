using SkiaSharp;

namespace rMultiplatform
{
	public abstract class ASmartAxisPair : ASmartElement
	{
		public ASmartData Parent;
		public ASmartAxis Horizontal, Vertical;
		public abstract SKMatrix Transform(SKSize dimension);
		public SKRect AxisClip(SKSize dimension)
		{
			return new SKRect(Horizontal.AxisStart(dimension.Width), Vertical.AxisStart(dimension.Height), Horizontal.AxisEnd(dimension.Width), Vertical.AxisEnd(dimension.Height));
		}

		public bool EnableTouchVertical = false;
		public bool EnableTouchHorizontal = true;

		public void Reset()
		{
			Horizontal.Range.Reset();
			Vertical.Range.Reset();
		}

		public void Zoom(float dx, float dy, float cx, float cy)
		{
			if (EnableTouchHorizontal)
				Horizontal.Zoom(dx, cx);

			if (EnableTouchVertical)
				Vertical.Zoom(dy, cy);

			Parent.Parent.InvalidateSurface();
		}
		public void Zoom(SKPoint Amount, SKPoint About)
		{
			Zoom(Amount.X, Amount.Y, About.X, About.Y);
		}
		public void Pan(float dx, float dy)
		{
			if (EnableTouchHorizontal)
				Horizontal.Pan(dx);

			if (EnableTouchVertical)
				Vertical.Pan(dy);

			Parent.Parent.InvalidateSurface();
		}
		public void Pan(SKPoint Amount)
		{
			Pan(Amount.X, Amount.Y);
		}

        public float VerticalPadding
        {
            get; set;
        } = 0.1f;
        public float MinimumPadding
        {
            get; set;
        } = 0.000001f;

		public void Set(SKRect Boundary)
		{
            //Prevents zero size axis and padds by a ratio defined by Vertical Padding
            var padding_vert = Boundary.Height * VerticalPadding;
            if (padding_vert <= MinimumPadding)
                padding_vert = MinimumPadding;

            Horizontal.Range.SetBoundary(Boundary.Left, Boundary.Right);
			Vertical.Range.SetBoundary(Boundary.Top - padding_vert, Boundary.Bottom + padding_vert);
		}

		public abstract void Draw(SKCanvas canvas, SKSize dimension, SKSize view);
	}

	public class SmartAxisPair : ASmartAxisPair
	{
		public SmartAxisPair(ASmartAxis pHorizontal, ASmartAxis pVertical)
		{
			Horizontal = pHorizontal;

			Vertical = pVertical;
		}
		public override SKMatrix Transform(SKSize dimension)
		{
			var horz_map	= Horizontal.CoordinateFromValue(dimension.Width);
			var vert_map	= Vertical.CoordinateFromValue(dimension.Height);
			return Map.CreateMatrix(horz_map, vert_map);
		}
		public override void Draw(SKCanvas canvas, SKSize dimension, SKSize view)
		{
			Horizontal.Position = Padding.BottomPosition(dimension.Height);
			Vertical.Position   = Padding.LeftPosition  (dimension.Width);
			Horizontal.Draw(canvas, dimension, view);
			Vertical.Draw(canvas, dimension, view);
		}
	}
}