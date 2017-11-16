using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	class SmartTitle : ASmartElement
	{
		#region PARENT_FUNCTIONS
		SmartChart Parent;
		private void InvalidateSurface()
		{
			Parent.InvalidateSurface();
		}
		#endregion

		public string Title { get; set; } = "";
		public SKPaint TitlePaint = MakeDefaultPaint(Globals.TextColor, 2, Globals.TitleFontSize, Globals.Typeface);
		public enum LabelPosition
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}
		public LabelPosition Position = LabelPosition.TopRight;

        public void Draw(SKCanvas canvas, SKSize dimension, SKSize view)
        {
            float x = 0, y = 0;

            //Handles different DPI
            (var scalex, var scaley) = SmartDPI.GetScale(canvas, dimension, view);
            var temp_paint = MajorPaint(scaley);
            (var dx, var dy) = MeasureText(Title, temp_paint);

            switch (Position)
			{
				case LabelPosition.TopLeft:
					x = Padding.LeftPosition   (dimension.Width)   + dy;
					y = Padding.TopPosition    (dimension.Height)  + dy;
					break;
				case LabelPosition.TopRight:
					x = Padding.RightPosition  (dimension.Width)   - dy - dx;
					y = Padding.TopPosition    (dimension.Height)  + dy;
					break;
				case LabelPosition.BottomLeft:
					x = Padding.LeftPosition   (dimension.Width)   + dy;
					y = Padding.BottomPosition (dimension.Height)  - dy;
					break;
				case LabelPosition.BottomRight:
					x = Padding.RightPosition  (dimension.Width)   - dy - dx;
					y = Padding.BottomPosition (dimension.Height)  - dy;
					break;
			};

			canvas.DrawText(Title, x, y, temp_paint);
		}
	}
}
