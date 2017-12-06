using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using static rMultiplatform.GeneralControl;
using System;
using System.Diagnostics;

namespace rMultiplatform
{
    public abstract class ASmartElement
    {
        public static SKPaint MakeDefaultPaint(Color pColor, float pStrokeWidth, float pFontSize, SKTypeface pTypeface, bool Dotted = false, bool IsStroke = false)
        {
            var output = new SKPaint()
            {
                Color = pColor.ToSKColor(),
                StrokeWidth = pStrokeWidth,
                Typeface = pTypeface,
                TextSize = pFontSize,
                StrokeCap = SKStrokeCap.Round,
                BlendMode = SKBlendMode.Src,
                IsStroke = IsStroke,
                IsAntialias = true
            };
            if (Dotted)
            {
                output.ColorFilter = SKColorFilter.CreateBlendMode(pColor.ToSKColor(), SKBlendMode.Dst);
                output.PathEffect = SKPathEffect.CreateDash(new[] { 1f, 1f }, 0);
            }
            return output;
        }

        private static SKPaint _MajorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MajorFontSize,  Globals.Typeface);
        private static SKPaint _MinorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface);
        private static SKPaint _GridPaint   = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface, Dotted: true);
        private static SKPaint _MaskPaint   = MakeDefaultPaint(Globals.BackgroundColor, 1,  Globals.MinorFontSize,  Globals.Typeface);

        public static SKPaint ScaledPaint   (float scale, SKPaint paint)
        {
            var cpy = paint.Clone();
            cpy.TextSize *= scale;
            return cpy;
        }
        public static SKPaint MajorPaint    (float scale) => ScaledPaint(scale, _MajorPaint);
        public static SKPaint MinorPaint    (float scale) => ScaledPaint(scale, _MajorPaint);

        public static SKPaint MaskPaint
        {
            get
            {
                return _MaskPaint;
            }
            private set
            {
                _MaskPaint = value;
            }
        }
        public static SKPaint GridPaint(float scale)
        {
            var temp = ScaledPaint(scale, _GridPaint);
            temp.ColorFilter = SKColorFilter.CreateBlendMode(temp.Color, SKBlendMode.Dst);
            temp.PathEffect = SKPathEffect.CreateDash(new[] { 2 * scale, 2 * scale }, 0);
            return temp;
        }

        public static float MinorTextSize => Globals.MinorFontSize;
        public static float MajorTextSize => Globals.MajorFontSize;

        public static (float x, float y) MeasureText(string Input, SKPaint Paint)
        {
            SKRect temp = new SKRect(0, 0, 0, 0);
            Paint.MeasureText(Input, ref temp);
            //Compensates for cropping of pixels
            return (temp.Width + 2, temp.Height + 2);
        }
        public static float SpaceWidth(SKPaint Paint) => (Paint.MeasureText(" "));

        public static SmartPadding Padding { get; private set; } = new SmartPadding(0.05f, 0, 0.1f, 0);
		public ASmartElement(){}
	}

	public class SmartChart : GeneralRenderedView
    {
		private SmartData   Data;
		private SmartTitle  _Title = new SmartTitle() { Title = "" };

		#region EVENTS
		public event EventHandler Clicked;
		#endregion
		
		#region TOUCHSCREEN
		private Touch mTouch;
		private void MTouch_Press   (object sender, TouchActionEventArgs args)  {}
		private void MTouch_Hover   (object sender, TouchActionEventArgs args)  {}
		private void MTouch_Release (object sender, TouchActionEventArgs args)  {}
		private void SetupTouch()
		{
			//Add the gesture recognizer 
			mTouch = new Touch();
			mTouch.Tap	    += MTouch_Tap;
			mTouch.Pressed  += MTouch_Press;
			mTouch.Hover	+= MTouch_Hover;
			mTouch.Released += MTouch_Release;
			mTouch.Pinch	+= MTouch_Pinch;
			mTouch.Pan	    += MTouch_Pan;
            mTouch.Scroll   += MTouch_Scroll;
			Effects.Add(mTouch);
		}


        private void MTouch_Scroll(object sender, ScrollActionEventArgs args)
        {
            var dist = 1f + (float)args.Steps / (720f);

            var zoomX = dist;
            var zoomY = 0;
            var Center = args.About;

            Data.Axis.Zoom(zoomX, zoomY, (float)Center.X, (float)Center.Y);
        }
        private void MTouch_Tap(object sender, Touch.TouchTapEventArgs args)
		{
			Clicked?.Invoke(sender, EventArgs.Empty);
		}
		private void MTouch_Pan(object sender, TouchPanActionEventArgs args)
		{
			Data.Axis.Pan((float)args.Dx, (float)args.Dy);
		}
		private void MTouch_Pinch(object sender, TouchPinchActionEventArgs args)
		{
			var zoomX = (float)args.Pinch.ZoomX;
			var zoomY = (float)args.Pinch.ZoomY;
			var Center = args.Pinch.Center;
			Data.Axis.Zoom(zoomX, zoomY, (float)Center.X, (float)Center.Y);
		}
		#endregion
        
		public void SaveCSV()
		{
			Files.SaveFile(Data.GetCSV());
		}
		public string Title
		{
			get
			{
				return _Title.Title;
			}
			set
			{
				_Title.Title = value;
			}
		}

        public override void PaintSurface(SKCanvas canvas, SKSize dimension, SKSize viewsize)
        {
            canvas.Clear(BackgroundColor.ToSKColor());
            (var x1, var y1, var x2, var y2) = 
                ASmartElement.Padding.GetHorizontalLine(dimension.Width, 10);

            _Title.Draw(canvas, dimension, viewsize);
            Data.Draw(canvas, dimension, viewsize);
        }
        public SmartChart(SmartData pData)
        {
            Data = pData;
            Data.Parent = this;
            SetupTouch();
            IsVisible = true;
        }
	}
}
